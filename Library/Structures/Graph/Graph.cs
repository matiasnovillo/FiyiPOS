using System.Data;

namespace FiyiPOS.Library.Structures.Graph
{
    public class Graph
    {
        //Minimum properties to run
        private Graph NextNodeList { get; set; } = new();
        private Graph FirstNodeList { get; set; } = new();

        //Add-ons for NodeList
        public string? Data1 { get; set; }
        public int Data2 { get; set; }
        public decimal Data3 { get; set; }

        public void Add(Graph NewNodeList)
        {
            try
            {
                if (FirstNodeList == null) 
                { 
                    FirstNodeList = NewNodeList; 
                }
                else if (Validator.CompareStrings(NewNodeList.Data1 ?? "VACIO", FirstNodeList.Data1 ?? "VACIO") == 'B')
                {
                    NewNodeList.NextNodeList = FirstNodeList;
                    FirstNodeList = NewNodeList;
                }
                else
                {
                    var AuxFirstNodeList = FirstNodeList;

                    if (AuxFirstNodeList.NextNodeList != null)
                    {
                        while (Validator.CompareStrings
                            (AuxFirstNodeList.NextNodeList.Data1 ?? "VACIO", NewNodeList.Data1 ?? "VACIO") == 'B')
                        {
                            AuxFirstNodeList = AuxFirstNodeList.NextNodeList;

                            if (AuxFirstNodeList.NextNodeList == null) { break; }
                        }
                    }

                    NewNodeList.NextNodeList = AuxFirstNodeList.NextNodeList ?? new() { Data1 = "VACIO"};

                    AuxFirstNodeList.NextNodeList = NewNodeList;
                }
            }
            catch (Exception) { throw; }
        }

        public DataTable ToDataTable()
        {
            try
            {
                DataTable DataTable = new();
                DataTable.Columns.Add("Data1", typeof(string));
                DataTable.Columns.Add("Data2", typeof(int));
                DataTable.Columns.Add("Data3", typeof(decimal));
                Graph AuxFirstNodeList = FirstNodeList ?? new();

                while (AuxFirstNodeList != null)
                {
                    DataTable.Rows.Add(AuxFirstNodeList.Data1, AuxFirstNodeList.Data2, AuxFirstNodeList.Data3);
                    AuxFirstNodeList = AuxFirstNodeList.NextNodeList ?? new();
                }
                return DataTable;
            }
            catch (Exception) { throw; }
        }
    }
}
