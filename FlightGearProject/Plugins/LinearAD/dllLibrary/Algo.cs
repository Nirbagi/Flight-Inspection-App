using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dllLibrary
{
    //Line

    public class Point
    {
        public float x;
        public float y;
        public Point()
        {
            x = 0;
            y = 0;
        }
        public Point(float a, float b)
        {
            x = a;
            y = b;
        }
    }

    public class Line
    {
        public float a, b;
        public Line()
        {
            a = b = 0;
        }

        public Line(float x, float y)
        {
            a = x;
            b = y;

        }
        public float f(float x)
        {
            return a * x + b;
        }
    };

    public class AnomalyReport
    {
        public string description;
        public long timeStep;
        public float x;
        public float y;
        public AnomalyReport(string desc, long tS, float a, float b)
        {
            description = desc;
            timeStep = tS;
            x = a;
            y = b;
        }
    };

    public struct correlatedFeatures
    {
        public string feature1;
        public string feature2;  // names of the correlated features
        public float corrlation;
        public float threshold;
        public Line lin_reg;
        public List<Point> draw;
    }

    class util
    {
        public static float avg(float[] x, int size)
        {
            float sum = 0;
            for (int i = 0; i < size; sum += x[i], i++) ;
            return sum / size;
        }

        // returns the variance of X and Y
        public static float var(float[] x, int size)
        {
            float av = avg(x, size);
            float sum = 0;
            for (int i = 0; i < size; i++)
            {
                sum += x[i] * x[i];
            }
            return sum / size - av * av;
        }

        // returns the covariance of X and Y
        public static float cov(float[] x, float[] y, int size)
        {
            float sum = 0;
            for (int i = 0; i < size; i++)
            {
                sum += x[i] * y[i];
            }
            sum /= size;

            return sum - avg(x, size) * avg(y, size);
        }


        // returns the Pearson correlation coefficient of X and Y
        public static float pearson(float[] x, float[] y, int size)
        {
            float a = (float)cov(x, y, size);
            float b = (float)((Math.Sqrt(var(x, size)) * Math.Sqrt(var(y, size))));
            return a / b;
        }

        // performs a linear regression and returns the line equation
        public static Line linear_reg(ref Point[] points, int size)
        {

            float[] x = new float[size];
            float[] y = new float[size];
            for (int i = 0; i < size; i++)
            {
                x[i] = points[i].x;
                y[i] = points[i].y;
            }
            float a = cov(x, y, size) / var(x, size);
            float b = avg(y, size) - a * (avg(x, size));

            return new Line(a, b);
        }

        // returns the deviation between point p and the line equation of the points
        public static float dev(Point p, ref Point[] points, int size)
        {
            Line l = linear_reg(ref points, size);
            return dev(p, l);
        }

        // returns the deviation between point p and the line
        public static float dev(Point p, Line l)
        {
            float x = (float)(p.x - l.f(p.x));
            if (x < 0)
                x *= -1;
            return x;
        }

    }

    public class TimeSeries
    {
        public Dictionary<string, List<float>> data { set; get; }
        public List<string> features { set; get; }
        public int numOfFeature { set; get; }
        public int numOfSamples { set; get; }

        List<string> createFeatures(string filePath)
        {
            List<string> featureList = new List<string>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(filePath);

            if ((line = file.ReadLine()) != null) // check if file is not empty and read first line
            {
                string[] splitedLine = line.Split(','); // splite the first line, the feature line
                for (int i = 0; i < splitedLine.Length; i++)
                {
                    if (!featureList.Contains(splitedLine[i])) // the list does't contain the same string
                    {
                        featureList.Add(splitedLine[i]);
                    }
                    else
                    {
                        int t = 1;
                        string str = splitedLine[i] + t.ToString();
                        while (featureList.Contains(str))
                        {
                            t++;
                        }
                        featureList.Add(str);
                    }
                }
            }
            return featureList;
        }

        List<float> createVectorFromFile(int i, string filePath)
        {
            List<float> temp = new List<float>();
            string line;
            string[] splitedLine;
            System.IO.StreamReader file = new System.IO.StreamReader(filePath);
            line = file.ReadLine(); //first line is not relevant to the vectors
            while ((line = file.ReadLine()) != null)
            {
                splitedLine = line.Split(','); // splite the first line, the feature line
                temp.Add(float.Parse(splitedLine[i]));
            }
            return temp;
        }

        Dictionary<string, List<float>> createData(string filePath, int numOfSamples, List<string> features)
        {
            Dictionary<string, List<float>> data = new Dictionary<string, List<float>>();
            List<float> tempList = new List<float>();
            for (int i = 0; i < features.Count; i++)
            {
                tempList = createVectorFromFile(i, filePath);
                data.Add(features[i], tempList);
            }
            return data;
        }

        // CTOR
        public TimeSeries(string filePath)
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(filePath);

            if ((line = file.ReadLine()) == null) // file is empty
            {
                numOfFeature = 0;
                numOfSamples = 0;
            }
            else // file is not empty
            {
                features = createFeatures(filePath);
                numOfFeature = features.Count();
                numOfSamples = (System.IO.File.ReadLines(filePath).Count()) - 1;
                data = createData(filePath, numOfSamples, features);
            }
            file.Close();
        }
    }

    public class Algo
    {
        //Line

        public List<correlatedFeatures> cf;
        public List<String> cFString;
        public List<String> ar;
        public float threshold;

        public Algo()
        {
            cf = new List<correlatedFeatures>();
            cFString = new List<String>();
            ar = new List<String>();
            threshold = (float)0.9;
        }

        void setCorrelationThreshold(float t)
        {
            threshold = t;
        }

        float findThreshold(ref Point[] ps, int len, Line rl)
        {
            float max = 0;
            float d;
            for (int i = 0; i < len; i++)
            {
                d = Math.Abs(ps[i].y - rl.f((float)ps[i].x));
                if (d > max)
                    max = d;
            }
            return max;
        }

        Point[] toPoints(List<float> x, List<float> y)
        {
            int size = x.Count;
            Point[] ps = new Point[size];
            for (int i = 0; i < size; i++)
            {
                ps[i] = new Point(x[i], y[i]);
            }
            return ps;
        }

        List<Point> draw(Line line, float xMIN, float xMAX)
        {
            List<Point> pointlist = new List<Point>();
            float pointDist = (xMAX - xMIN) / 50;
            float yi;

            for (float i = xMIN; i <= xMAX; i += pointDist)
            {
                yi = line.f(i);
                pointlist.Add(new Point(i, yi));
            }
                return pointlist;
        }

        void learnHelper(ref TimeSeries ts, float p, string f1, string f2, ref Point[] ps,float xmin, float xmax)
        {
            if (p > threshold)
            {
                correlatedFeatures c;
                int len = ts.numOfSamples;
                c.feature1 = f1;
                c.feature2 = f2;
                c.corrlation = p; //pearson
                c.lin_reg = util.linear_reg(ref ps, len);
                c.threshold = (findThreshold(ref ps, len, c.lin_reg)) * (float)1.1; // 10% increase
                c.draw = draw(c.lin_reg,xmin,xmax);
                cFString.Add(c.feature1 + "," + c.feature2);
                cf.Add(c);
            }
        }

        public List<correlatedFeatures> getCF()
        {
            return cf;
        }

        public List<String> getcFString()
        {
            return cFString;
        }

        bool stringCompare(string s1,string s2)
        {
            if (String.Compare(s1, s2) == 0)
            {
                return true;
            }
            return false;
        }

        public Dictionary<String, List<String>> getDraw()
        {
            Dictionary<String, List<String>> drawPoints = new Dictionary<String, List<String>>();
            List<String> temp = new List<String>();
            string tuple = "";
            for (int i = 0; i < cf.Count; i++)
            {
                temp.Clear();
                tuple = cf[i].feature1 + "," + cf[i].feature2;
                for(int j = 0; j < cf[i].draw.Count; j++)
                {
                    temp.Add(cf[i].draw[j].x.ToString() + "," + cf[i].draw[j].y.ToString());
                }
                drawPoints.Add(tuple, temp);
            }
            return drawPoints;
        }

        float findXmin(List<float> fd)
        {
            float min = 0;
            if (fd != null)
            {
                min = fd[0];
                for(int i = 0; i < fd.Count; i++)
                {
                    if(fd[i] < min)
                    {
                        min = fd[i];
                    }
                }
            }
            return min;
        }

        public float findXmax(List<float> fd)
        {
            float max = 0;
            if (fd != null)
            {
                max = fd[0];
                for (int i = 0; i < fd.Count; i++)
                {
                    if (fd[i] > max)
                    {
                        max = fd[i];
                    }
                }
            }
            return max;
        }

        public void learnNormal(ref TimeSeries ts)
        {
            List<string> features = ts.features;
            int len = ts.numOfSamples;
            List<float> featureData;
            List<float> anotherFeatureData;
            float xmin, xmax;

            for (int i = 0; i < features.Count; i++)
            {
                string f1 = features[i];
                float max = 0;
                int jmax = 0;
                ts.data.TryGetValue(ts.features[i], out featureData);
                xmin = findXmin(featureData);
                xmax = findXmax(featureData);
                for (int j = i + 1; j < features.Count; j++)
                {
                    ts.data.TryGetValue(ts.features[j], out anotherFeatureData);
                    float p = Math.Abs(util.pearson(featureData.ToArray(), anotherFeatureData.ToArray(), len));
                    if (p > max)
                    {
                        max = p;
                        jmax = j;
                    }
                }
                string f2 = features[jmax];
                ts.data.TryGetValue(f1, out featureData);
                ts.data.TryGetValue(f2, out anotherFeatureData);
                Point[] ps = toPoints(featureData, anotherFeatureData);

                learnHelper(ref ts, max, f1, f2, ref ps,xmin, xmax);
            }
        }

        bool isAnomalous(float x, float y, correlatedFeatures cf)
        {
            float result = Math.Abs(y - cf.lin_reg.f(x));
            return result > cf.threshold;
        }


        public List<String> getAR()
        {
            return ar;
        }

        public void detect(ref TimeSeries ts)
        {
            ar = new List<String>();
            List<float> x;
            List<float> y;
            int lineNum;

            for (int i = 0; i < cf.Count; i++)
            {
                ts.data.TryGetValue(cf[i].feature1, out x);
                ts.data.TryGetValue(cf[i].feature2, out y);

                for (int j = 0; j < x.Count; j++)
                {
                    if (isAnomalous(x[j], y[j], cf[i]))
                    {
                        lineNum = j + 1;
                        String d = cf[i].feature1 + ","+ cf[i].feature2 + "," + (lineNum).ToString() + "," + x[j].ToString() + "," + y[j].ToString();
                        ar.Add(d);
                        //ar.Add(new AnomalyReport(cf[i].feature1,cf[i].feature2,j+1,x[j],y[j]);
                    }
                }
            }
        }

    }
}
