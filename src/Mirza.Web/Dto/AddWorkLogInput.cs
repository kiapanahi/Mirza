using System.Collections.Generic;

namespace Mirza.Web.Dto
{
    public class AddWorkLogInput
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}