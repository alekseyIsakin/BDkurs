using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DBcore;

namespace WpfApp1.Forms.Controls
{
    public class PublisherEventArgs : EventArgs
    {
        public PublisherEventArgs(Publisher _publ)
        {
            slectedPublisher = _publ;
        }
        public Publisher slectedPublisher { get; }
    }
}
