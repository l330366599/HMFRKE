using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMFRKE
{
    //门禁记录表的缓存
    public class MenjinjiluResult
    {
        public string MJJLID { get; set; }

        public string CardID { get; set; }

        public DateTime? entryTime { get; set; }

        public DateTime? departureTime { get; set; }

        public bool YX { get; set; }
    }
}
