namespace FlightGearProject.EventModels
{
    // Event to update Graphs Data
    public class GraphEvent
    {
        public string CurLine { get; set; }
        public int CsvLineIndex { get; set; }
        public bool FBFlag { get; set; }

        public GraphEvent(string acurLine, int index, bool fb)
        {
            CurLine = acurLine;
            CsvLineIndex = index;
            FBFlag = fb;
        }
    }
}
