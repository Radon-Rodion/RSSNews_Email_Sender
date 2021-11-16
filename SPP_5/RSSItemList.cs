using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPP_5
{
    public class RSSItemList : List<RSSItem>
    {
        new public bool Contains(RSSItem item)
        {
            foreach (RSSItem checkingItem in this)
            {
                if (item.Title == checkingItem.Title)
                {
                    return true;
                }
            }
            return false;
        }

        public RSSItem GetItem(string title)
        {
            foreach (RSSItem checkingItem in this)
            {
                if (title == checkingItem.Title)
                {
                    return checkingItem;
                }
            }
            return null;
        }
    }
}
