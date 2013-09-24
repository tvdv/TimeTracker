using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using LumenWorks.Framework.IO.Csv;
using TimeTracker.Annotations;
using TimeTracker.util;

namespace TimeTracker.Model
{
    class Model : INotifyPropertyChanged
    {
        private readonly string _storageDir;
        private readonly Dispatcher _dispatcher;
        private ModelState _state;
        private BulkAddObservableCollection<TimeEntry> _entries;
        ManualResetEvent _mreTagsLoaded;

        internal enum ModelState
        {
            Uninitialised,
            Loading,
            Loaded,
            Saving
        }

        public ModelState State
        {
            get { return _state; }
            set
            {
                if (value == _state) return;
                _state = value;
                OnPropertyChanged("State");
            }
        }

        public Model(string storageDir,Dispatcher dispatcher)
        {
            Tags=new ObservableCollection<Tag>();
            _entries = new BulkAddObservableCollection<TimeEntry>();
            _storageDir = storageDir;
            _dispatcher = dispatcher;
            State = ModelState.Uninitialised;
            _mreTagsLoaded = new ManualResetEvent(false);

        }

        private string EntryFile
        {
            get { return Path.Combine(_storageDir, "entries.csv"); }
        }
        private string TagFile
        {
            get { return Path.Combine(_storageDir, "tags.csv"); }
        }

        public bool BeginLoad()
        {
            if (State != ModelState.Uninitialised)
            {
                return false;
            }

            State = ModelState.Loading;
            ThreadPool.QueueUserWorkItem(LoadThread, EntryFile);

            return true;
        }
        
        void LoadTags(string file)
        {
            if (!File.Exists(file))
            {
                //no tags yet
                return;
            }
            using (var csv = new CsvReader(new StreamReader(file), false))
            {
                csv.DefaultParseErrorAction=ParseErrorAction.AdvanceToNextLine;
                //string[] headers = csv.GetFieldHeaders();
                while (csv.ReadNextRecord())
                {
                    int fieldCount = csv.FieldCount;
                    var t = new Tag();
                    t.Name = csv[0];
                    Tag.TagType type;
                    if (!Enum.TryParse(csv[1], true, out type))
                    {
                        //failed to read
                        t.Type = Tag.TagType.UserDefined;
                    }
                    else
                    {
                        t.Type = type;
                    }
                    t.PrimaryBillingCode = csv[2];
                    t.SecondaryBillingCode = csv[3];
                    
                    _dispatcher.BeginInvoke(new Action(() => this.Tags.Add(t)));
                }
            }

            _dispatcher.BeginInvoke(new Action(() => this._mreTagsLoaded.Set()));
         
        }
        void LoadThread(object state)
        {
            System.Diagnostics.Debug.WriteLine("Load Thread start"  + DateTime.Now.ToLongTimeString());
            _mreTagsLoaded.Reset();
            var file = state.ToString();

            List<TimeEntry> newEntries=new List<TimeEntry>();
            LoadTags(TagFile);

            //make sure all the tags are loaded (by the mainthread)
            _mreTagsLoaded.WaitOne();
                        
            if (File.Exists(file))
            {
                using (var csv = new CsvReader(new StreamReader(file), false))
                {
                    csv.DefaultParseErrorAction=ParseErrorAction.AdvanceToNextLine;
                    //string[] headers = csv.GetFieldHeaders();
                    while (csv.ReadNextRecord())
                    {
                        int fieldCount = csv.FieldCount;

                        var entry=new TimeEntry();
                        entry.Start = DateTime.Parse(csv[0]);
                        entry.End = DateTime.Parse(csv[1]);
                        entry.Note = csv[2];

                        for (int i = 3; i < fieldCount; i++)
                        {
                            if (String.IsNullOrWhiteSpace(csv[i])) break;
                            //find the tag
                            var tag = this.Tags.FirstOrDefault(z => z.Name.ToLower() == csv[i].ToLower());
                            if (tag == null)
                            {
                                //couldnt find it
                                //ignore for the moment
                                System.Diagnostics.Debug.WriteLine("Could not find tag \"" + csv[i] + "\"");
                            }
                            else
                            {
                                entry.Tags.Add(tag);
                            }

                        }
                        
                        newEntries.Add(entry);
                        

                    }
                }
            }
            _dispatcher.BeginInvoke(new Action(() => _entries.AddRange(newEntries)));
            
            System.Diagnostics.Debug.WriteLine("Load Thread done" + DateTime.Now.ToLongTimeString());
            _dispatcher.BeginInvoke(new Action(() => State = ModelState.Loaded));
            
        }

        public bool BeginSave()
        {
            if (State != ModelState.Loaded)
                return false;

            State = ModelState.Saving;

            ThreadPool.QueueUserWorkItem(SaveThread, EntryFile);

            return true;
        }


        void SaveTags(string file)
        {
            var fileBackup = file + ".bak";
            try
            {
                if (File.Exists(fileBackup))
                {
                    File.Delete(fileBackup);
                }

                if (File.Exists(file))
                {
                    File.Move(file, fileBackup);
                }
            }
            catch (Exception e)
            {
                //backup failed
                System.Diagnostics.Debug.WriteLine("Backup of file before saving failed." + e.Message + ", " + file);
            }

            var fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
            var tw = new StreamWriter(fs);

            foreach (var tag in this.Tags)
            {
                tw.Write("\"" + tag.Name + "\"");
                tw.Write(",");
                tw.Write(tag.Type);
                tw.Write(",");
                tw.Write("\""+tag.PrimaryBillingCode+"\"");
                tw.Write(",");
                tw.Write("\"" + tag.SecondaryBillingCode + "\"");
                tw.WriteLine("");
            }

            tw.Close();

        }

        void SaveThread(object state)
        {
            var file = state.ToString();

            SaveTags(TagFile);

            var fileBackup = file + ".bak";
            try
            {
                if (File.Exists(fileBackup))
                {
                    File.Delete(fileBackup);
                }

                if (File.Exists(file))
                {
                    File.Move(file, fileBackup);
                }
            }
            catch (Exception e)
            {
                //backup failed
                System.Diagnostics.Debug.WriteLine("Backup of file before saving failed." + e.Message + ", " + file);
           
            }

            var fs=new FileStream(file,FileMode.Create,FileAccess.Write,FileShare.None);
            var tw=new StreamWriter(fs);

            foreach (var timeEntry in this.Entries)
            {
                tw.Write(timeEntry.Start); //TODO: Format date/time correctly (tz agnostic?)
                tw.Write(",");
                tw.Write(timeEntry.End);
                tw.Write(",");
                tw.Write("\""+timeEntry.Note+"\"");
                tw.Write(",");
                foreach (var tag in timeEntry.Tags)
                {
                    tw.Write(tag.Name);
                    tw.Write(",");
                }

                tw.WriteLine("");
            }

            tw.Close();

            _dispatcher.BeginInvoke(new Action(() => State = ModelState.Loaded));
        }

        public BulkAddObservableCollection<TimeEntry> Entries
        {
            get { return _entries; }
            protected set { _entries = value; }
        }

        public ObservableCollection<Tag> Tags { get; protected set; }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
