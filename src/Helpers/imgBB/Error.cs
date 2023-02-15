namespace imgBBUploader
{
    public class Error
    {
        public int status_code { get; set; }
        public Detail error { get; set; }
        public string status_txt { get; set; }

        public class Detail
        {
            public string message { get; set; }
            public int code { get; set; }
            public string context { get; set; }
        }
    }
}