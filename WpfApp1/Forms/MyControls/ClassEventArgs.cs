using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.DBcore;

namespace WpfApp1.Forms.MyControls
{
    public class GameFilterEventArg : EventArgs 
    {
        public List<Game> newGames;
        public GameFilterEventArg(List<Game> _newGames)
        {
            newGames = _newGames;
        }

    }
    public class PublisherEventArgs : EventArgs
    {
        public PublisherEventArgs(Publisher _publ)
        {
            slectedPublisher = _publ;
        }
        public Publisher slectedPublisher { get; }
    }
}
