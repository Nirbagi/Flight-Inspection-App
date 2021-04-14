using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dllLibrary
{
    //Circle
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

    public class Circle
    {
        public Point center = new Point();
        public float radius;
        public Circle(Point c, float r)
        {
            center = c;
            radius = r;
        }
        public Circle()
        {
            center = new Point(0,0);
            radius = 0;
        }
    }

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

    public class minCircle
    {
        Circle lastCalculate = new Circle();

        public static float Dist(Point a, Point b)
        {
            double x2 = Math.Pow((a.x - b.x), 2.0);
            double y2 = Math.Pow((a.y - b.y), 2.0);
            x2 += y2;
            return (float)Math.Sqrt(x2);
        }

        public static Circle from2points(Point a, Point b)
        {
            float x = (a.x + b.x) / 2;
            float y = (a.y + b.y) / 2;
            float r = ((float)Dist(a, b)) / 2;
            return (new Circle(new Point(x, y), r));
        }

        public static Circle from3Points(Point a, Point b, Point c)
        {
            // find the circumcenter of the triangle a,b,c

            Point mAB = new Point((a.x + b.x) / 2, (a.y + b.y) / 2); // mid point of line AB
            float slopAB = (b.y - a.y) / (b.x - a.x); // the slop of AB
            float pSlopAB = -1 / slopAB; // the perpendicular slop of AB
            // pSlop equation is:
            // y - mAB.y = pSlopAB * (x - mAB.x) ==> y = pSlopAB * (x - mAB.x) + mAB.y

            Point mBC = new Point((b.x + c.x) / 2, (b.y + c.y) / 2); // mid point of line BC
            float slopBC = (c.y - b.y) / (c.x - b.x); // the slop of BC
            float pSlopBC = -1 / slopBC; // the perpendicular slop of BC
            // pSlop equation is:
            // y - mBC.y = pSlopBC * (x - mBC.x) ==> y = pSlopBC * (x - mBC.x) + mBC.y


            float x = (-pSlopBC * mBC.x + mBC.y + pSlopAB * mAB.x - mAB.y) / (pSlopAB - pSlopBC);
            float y = pSlopAB * (x - mAB.x) + mAB.y;
            Point center = new Point(x, y);

            //ORIGINAL LINE: float R=dist(center,a);
            float R = (float)Dist(center, a);
            return (new Circle(center, R));
        }
        
        static Circle trivial(List<Point> P)
        {
            if (P.Count == 0)
                return new Circle(new Point(0, 0), 0);
            else if (P.Count == 1)
                return new Circle(P[0], 0);
            else if (P.Count == 2)
                return from2points(P[0], P[1]);

            // maybe 2 of the points define a small circle that contains the 3ed point
            Circle c = from2points(P[0], P[1]);
            if (Dist(P[2], c.center) <= c.radius)
                return c;
            c = from2points(P[0], P[2]);
            if (Dist(P[1], c.center) <= c.radius)
                return c;
            c = from2points(P[1], P[2]);
            if (Dist(P[0], c.center) <= c.radius)
                return c;
            // else find the unique circle from 3 points
            return from3Points(P[0], P[1], P[2]);
        }

        Circle welzl(ref Point[] P, List<Point> R, int n)
        {
            if ((n == 0)||(R.Count == 3))
            {
                lastCalculate = trivial(R);
                return trivial(R);
            }

            if (R.Count < 3)
            {
                Random random = new Random();
                int randomNum = random.Next();
                int i = randomNum % n;
                Point p = P[i];
                P[n - 1] = Interlocked.Exchange(ref P[i], P[n - 1]);
                Circle c = welzl(ref P, R, n - 1);

                if ((Dist(p, c.center)) <= c.radius)
                {
                    lastCalculate = c;
                    return c;
                }

                R.Add(p);
                return welzl(ref P, R, n - 1);
            }
            else
            {
                return lastCalculate;
            }
        }

        public Circle findMinCircle(ref Point[] points, int size)
        {
            List<Point> R = new List<Point>();
            return (welzl(ref points, R, size));
        }

    }

    public struct correlatedFeatures
    {
        public string feature1;
        public string feature2;  // names of the correlated features
        public float corrlation;
        public float threshold; //radius
        public Point center;
        public List<Point> draw;
        //public int isPassedPrearsonThreshold;
        //public Line lin_reg;

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
        //Circle
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

        float findThreshold(ref Point[] ps, int len, Point c)
        {
            float max = 0;
            float d;
            for (int i = 0; i < len; i++)
            {
                d = minCircle.Dist(ps[i],c);
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

        Point setCenter(ref Point[] points, int size)
        {
            minCircle m = new minCircle();
            Circle c = m.findMinCircle(ref points, size);
            return c.center;
        }


        List<Point> draw(Point center, float radius )
        {
            List<Point> pointlist = new List<Point>();
            float xMIN = center.x - radius;
            float xMAX = center.x + radius;
            float pointDist = radius / 10; //change num of points
            float y1, y2, xD;
            float radiusD = (float)Math.Pow(radius,2);
            for(float x =(-radius); x<= radius;x += pointDist)
            {
                xD = (float)Math.Pow(x, 2);
                y1 = (float)Math.Sqrt(radiusD - xD);
                y2 = (-1) * y1;
                y1 += center.y;
                y2 += center.y;

                pointlist.Add(new Point(x+center.x, y1));
                pointlist.Add(new Point(x+center.x, y2));

            }
            return pointlist;
        }

        void learnHelper(ref TimeSeries ts, float p, string f1, string f2, ref Point[] ps)
        {
            if (p > threshold)
            {
                correlatedFeatures c;
                int len = ts.numOfSamples;
                c.feature1 = f1;
                c.feature2 = f2;
                c.corrlation = p; //pearson
                c.center = setCenter(ref ps, len);
                c.threshold = (findThreshold(ref ps, len, c.center)) * (float)1.1; // 10% increase
                c.draw = draw(c.center, c.threshold);
                cFString.Add(c.feature1 + "," + c.feature2);
                cf.Add(c);
            }
            else if (p > 0.5 && p < threshold)
            {
                minCircle m = new minCircle();
                Circle cl = m.findMinCircle(ref ps, ts.numOfSamples);
                correlatedFeatures c;
                c.feature1 = f1;
                c.feature2 = f2;
                c.corrlation = p;
                c.threshold = cl.radius * (float)1.1; // 10% increase
                c.center = cl.center;
                c.draw = draw(c.center, c.threshold);
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

        bool stringCompare(string s1, string s2)
        {
            if (String.Compare(s1, s2) == 0)
            {
                return true;
            }
            return false;
        }

        public List<string> getDraw(string f1, string f2)
        {
            List<string> drawPoints = new List<string>();
            for (int i = 0; i < cf.Count; i++)
            {
                if ((stringCompare(cf[i].feature1, f1) && stringCompare(cf[i].feature2, f2))
                    || (stringCompare(cf[i].feature2, f1) && stringCompare(cf[i].feature1, f2))) //check the first feature name
                {
                    for (int j = 0; j < cf[i].draw.Count; j++)
                    {

                        drawPoints.Add(cf[i].draw[j].x.ToString() + "," + cf[i].draw[j].y.ToString());
                    }
                }
            }
            return drawPoints;

        }

        public void learnNormal(ref TimeSeries ts)
        {
            List<string> features = ts.features;
            int len = ts.numOfSamples;
            List<float> featureData;
            List<float> anotherFeatureData;

            for (int i = 0; i < features.Count; i++)
            {
                string f1 = features[i];
                float max = 0;
                int jmax = 0;
                ts.data.TryGetValue(ts.features[i], out featureData);
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

                learnHelper(ref ts, max, f1, f2, ref ps);
            }
        }

        bool isAnomalous(float x, float y, correlatedFeatures c)
        {
            Point p = new Point(x, y);
            float result = minCircle.Dist(p,c.center);
            return ((c.corrlation >= threshold) && (result > c.threshold)) ||
                    ((c.corrlation > 0.5) && (c.corrlation < threshold) && (result > c.threshold));
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
                        String d = cf[i].feature1 + "," + cf[i].feature2 + "," + (lineNum).ToString() + "," + x[j].ToString() + "," + y[j].ToString();
                        ar.Add(d);
                        //ar.Add(new AnomalyReport(cf[i].feature1 + " - " + cf[i].feature2, (j + 1), x[j], y[j]));
                    }
                }
            }
        }

    }
}
