using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeTracker.TimeEntryView;
using TimeTracker.Model;
using System.Collections.ObjectModel;

namespace TimeTrackerTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestTimeEntryEditViewModel()
        {
            var tagList = new ObservableCollection<Tag>();
            tagList.Add(new Tag() { Name = "a" ,Type=Tag.TagType.BillingCode});
            tagList.Add(new Tag() { Name = "b", Type = Tag.TagType.UserDefined });
            tagList.Add(new Tag() { Name = "c", Type = Tag.TagType.BillingCode});

            var te = new TimeEntry();
            te.Tags.Add(tagList[0]);
            te.Tags.Add(tagList[1]);
            var vm = new TimeEntryEditViewModel(te,tagList,null);

            Assert.AreEqual(2, vm.AssociatedTags.Count);
        }
    }
}
