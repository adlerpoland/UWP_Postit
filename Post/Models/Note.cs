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
    public class NoteManager
    {
        public static ObservableCollection<Note> GetNotes()
        {
            var notes = new ObservableCollection<Note>();

            notes.Add(new Note { id = 1, header = "Clean wardrobe", content = "Clean that ugly wardrobe in bedroom", date = new DateTime(2017, 05, 28) });
            notes.Add(new Note { id = 2, parentid = 1, header = "Fix car", content = "Take some time to fix your mother's car", date = new DateTime(2017, 05, 29) });
            notes.Add(new Note { id = 3, parentid = 2, header = "End UWP project", content = "End your uwp project", date = new DateTime(2017, 06, 02) });
            notes.Add(new Note { id = 4, header = "Buy camera", content = "Find some cool camera for your trip", date = new DateTime(2017, 05, 28) });
            notes.Add(new Note { id = 5, header = "Dad birthday!", content = "Call him", date = new DateTime(2017, 06, 04) });
            notes.Add(new Note { id = 6, header = "Get mail", content = "Take that mail from an office", date = new DateTime(2017, 06, 05) });
            notes.Add(new Note { id = 7, header = "Return stolen bike to Jimmy", content = "Jimmy wants his bike", date = new DateTime(2017, 06, 05) });
            notes.Add(new Note { id = 8, parentid = 7,header = "This is the longest note available", content = "Lorum ipsum blah blah blah 123 flyingflyingasdbnasdajnsdaasdf test dasdas gasda gasd agasd asdadw dawd adasd asd a dsasda sdas dasd asdasdasddddsas h hh fg erw", date = new DateTime(2017, 06, 05) });

            return notes;
        }
    }
}
