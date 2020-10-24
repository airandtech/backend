using System.Collections.Generic;

namespace AirandWebAPI.Models.Direction
{
    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }

        public Distance(){}
        public Distance(string text, int value){
            this.text = text;
            this.value = value;
        }
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }

        public Duration() {}
        public Duration(string text, int value) {
            this.text = text;
            this.value = value;
        }
    }

    public class Element
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string status { get; set; }
    }

    public class Row
    {
        public List<Element> elements { get; set; }
    }

    public class DistanceMatrixResponse
    {
        public List<string> destination_addresses { get; set; }
        public List<string> origin_addresses { get; set; }
        public List<Row> rows { get; set; }
        public string status { get; set; }
    }
}