using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVR_WPF
{
    class ListViewSearchStudent
    {
        public int i
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string FirstName
        {
            get;
            set;
        }

        public static ObservableCollection<ListViewSearchStudent> getList()
        {
            var list = new ObservableCollection<ListViewSearchStudent>();
            return list;
        }

    }
}
