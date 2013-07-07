using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeTracker.Model;

namespace TimeTracker.TimeEntryView
{
    public class TimeEntryTagAssociationViewModel
    {
        private readonly Tag _tag;
        private readonly ICommand _deleteAssociationCommand;

        public TimeEntryTagAssociationViewModel(Tag tag,ICommand deleteAssociationCommand)
        {
            _tag = tag;
            _deleteAssociationCommand = deleteAssociationCommand;
        }


        public Tag Tag
        {
            get { return _tag; }
        }

        public ICommand DeleteAssociationCommand
        {
            get { return _deleteAssociationCommand; }
        }
    }
}
