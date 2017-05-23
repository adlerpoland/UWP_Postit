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
                    n.header = n.header + " #" + i.ToString();
                    collection.Add(n);
                }
                else if(n.parentid == id)
                {
                    i++;
                    n.header = n.header + " #" + i.ToString();
                    collection.Add(n);
                }
                
            }

            return collection;
        }


        public static List<Note> GetNoteList()
        {
            var list = new List<Note>();

            list.Add(new Note { id = 1, parentid = 0, header = "Clean stuff", content = "Clean that ugly wardrobe in bedroom", date = new DateTime(2017, 06, 06) });
            list.Add(new Note { id = 2, parentid = 1, header = "Fix car", content = "Take some time to fix your mother's car", date = new DateTime(2017, 06, 04) });
            list.Add(new Note { id = 3, parentid = 1, header = "End project", content = "End your uwp project", date = new DateTime(2017, 06, 03) });
            list.Add(new Note { id = 4, parentid = 0, header = "Buy camera", content = "Find some cool camera for your trip", date = new DateTime(2017, 05, 28) });
            list.Add(new Note { id = 5, parentid = 0, header = "Birthday!", content = "Call your dad!", date = new DateTime(2017, 06, 04) });
            list.Add(new Note { id = 6, parentid = 0, header = "Get mail", content = "Take that mail from an office", date = new DateTime(2017, 06, 04) });
            list.Add(new Note { id = 7, parentid = 0, header = "Return bike", content = "Jimmy wants his bike back", date = new DateTime(2017, 06, 05) });
            list.Add(new Note { id = 8, parentid = 7, header = "Long note", content = "Lorum ipsum blah blah blah 123 flyingflyingasdbnasdajnsdaasdf test dasa sdas dasd asdasdasddddsas h hh fg erwa asd asdaa", date = new DateTime(2017, 06, 04) });
            list.Add(new Note { id = 9, parentid = 1, header = "Clean garage", content = "Today you're gonna clean garage :)", date = new DateTime(2017, 06, 02) });
            list.Add(new Note { id = 10, parentid = 1, header = "Clean bedrm", content = "Bedroom today ;o", date = new DateTime(2017, 06, 02) });
            list.Add(new Note { id = 11, parentid = 1, header = "Clean wc", content = "Yes, that part of house today", date = new DateTime(2017, 06, 01) });
            list.Add(new Note { id = 12, parentid = 1, header = "Clean garden", content = "Finally something better to do", date = new DateTime(2017, 05, 30) });
            list.Add(new Note { id = 13, parentid = 1, header = "Free time", content = "Today you can enjoy your free time", date = new DateTime(2017, 05, 28) });
            list.Add(new Note { id = 14, parentid = 7, header = "Pay Bills", content = "Time to pay bills :(", date = new DateTime(2017, 06, 03) });
            list.Add(new Note { id = 15, parentid = 0, header = "Breaking Bad", content = "Remember to watch BB!", date = new DateTime(2017, 06, 10) });
            list.Add(new Note { id = 16, parentid = 15, header = "Shameless", content = "New season starting!", date = new DateTime(2017, 06, 09) });
            list.Add(new Note { id = 17, parentid = 15, header = "Walking Dead", content = "Hershel why did you died?", date = new DateTime(2017, 06, 01) });
            list.Add(new Note { id = 18, parentid = 7, header = "Pay Taxes", content = "God damn it!", date = new DateTime(2017, 06, 03) });
            list.Add(new Note { id = 19, parentid = 0, header = "Back to home", content = "Fuuu", date = new DateTime(2017, 06, 02) });
            list.Add(new Note { id = 20, parentid = 19, header = "Trip to LA ", content = "Pack your stuff!", date = new DateTime(2017, 05, 27) });


            return list;
        }
    }
}
