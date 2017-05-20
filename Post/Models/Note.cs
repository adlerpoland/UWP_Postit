using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Models
{
    public class Note
    {
        public int id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public String header { get; set; }
        public String content { get; set; }
        public DateTime date { get; set; }
        public int parentid { get; set; }
    }

    public class NoteCollecton
    {
        public ObservableCollection<Note> NoteCollection { get; set; }
    }

    public class NoteManager
    {
        public static bool isFirstNote(int id)
        {
            var list = GetNoteList();

            foreach (Note n in list)
            {
                if (n.id == id && n.parentid == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static ObservableCollection<Note> GetSingleCollection(int id)
        {
            var collection = new ObservableCollection<Note>();

            var list = GetNoteList();

            int i = 1;
            foreach (Note n in list)
            {             
                if(n.id == id && n.parentid == 0)
                {
                    n.header = n.header + " " + i.ToString() + "#";
                    collection.Add(n);
                }
                else if(n.parentid == id)
                {
                    i++;
                    n.header = n.header + " " + i.ToString() + "#";
                    collection.Add(n);
                }
                
            }

            return collection;
        }


        public static List<Note> GetNoteList()
        {
            var list = new List<Note>();

            list.Add(new Note { id = 1, parentid = 0, header = "Clean wardrobe", content = "Clean that ugly wardrobe in bedroom", date = new DateTime(2017, 05, 28) });
            list.Add(new Note { id = 2, parentid = 1, header = "Fix car", content = "Take some time to fix your mother's car", date = new DateTime(2017, 05, 29) });
            list.Add(new Note { id = 3, parentid = 1, header = "End UWP project", content = "End your uwp project", date = new DateTime(2017, 06, 02) });
            list.Add(new Note { id = 4, parentid = 0, header = "Buy camera", content = "Find some cool camera for your trip", date = new DateTime(2017, 05, 28) });
            list.Add(new Note { id = 5, parentid = 0, header = "Dad birthday!", content = "Call him", date = new DateTime(2017, 06, 04) });
            list.Add(new Note { id = 6, parentid = 0, header = "Get mail", content = "Take that mail from an office", date = new DateTime(2017, 06, 05) });
            list.Add(new Note { id = 7, parentid = 0, header = "Return stolen bike to Jimmy", content = "Jimmy wants his bike", date = new DateTime(2017, 06, 05) });
            list.Add(new Note { id = 8, parentid = 7, header = "This is the longest note available", content = "Lorum ipsum blah blah blah 123 flyingflyingasdbnasdajnsdaasdf test dasdas gasda gasd agasd asdadw dawd adasd asd a dsasda sdas dasd asdasdasddddsas h hh fg erw", date = new DateTime(2017, 06, 05) });

            return list;
        }

        public static ObservableCollection<int> GetNotesCollection()
        {
            var list = GetNoteList();

            var collection = new ObservableCollection<int>();

            foreach(Note n in list)
            {
                collection.Add(n.id);
            }

            return collection;
        }
    }
}
