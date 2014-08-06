using CommerceServer.Core.Catalog;
using KeithLink.Svc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT = KeithLink.Svc.Impl.RequestTemplates;

namespace KeithLink.Svc.Impl.Repository
{
    public class MockListRepositoryImpl : IListRepository
    {
        public static List<UserList> sampleList = new List<UserList>()
                {
                    new UserList() {
                        Name = "Sample List A",
                        ListId = Guid.NewGuid(),
                        Items = new List<ListItem>() {
                            new ListItem() {
                                ListItemId = Guid.NewGuid(),
                                ItemNumber = "284569",
                                Label = "This is a label",
                                Position = 0
                            },
                            new ListItem() {
                                ListItemId = Guid.NewGuid(),
                                ItemNumber = "287100",
                                Position = 1
                            }

                        }
                    },
                    new UserList() {
                        Name = "Sample List B",
                        ListId = Guid.NewGuid(),
                        Items = new List<ListItem>() {
                            new ListItem() {
                                ListItemId = Guid.NewGuid(),
                                ItemNumber = "287302",
                                Position = 0
                            },
                            new ListItem() {
                                ListItemId = Guid.NewGuid(),
                                ItemNumber = "287770",
                                Label = "Test Label",
                                Position = 1
                            },
                            new ListItem() {
                                ListItemId = Guid.NewGuid(),
                                ItemNumber = "287402",
                                Position = 2
                            }

                        }
                    }
                };

        public List<UserList> ReadAllLists()
        {
            return sampleList;
        }


        public UserList ReadList(Guid listId)
        {
            return sampleList.Where(l => l.ListId.Equals(listId)).FirstOrDefault();
        }

        public Guid CreateList(UserList list)
        {
            sampleList.Add(list);
            return list.ListId;
        }

        public void DeleteList(Guid listId)
        {
            sampleList.RemoveAll(l => l.ListId.Equals(listId));
        }


        public void DeleteItem(UserList list, Guid itemId)
        {
            list.Items.RemoveAll(i => i.ListItemId.Equals(itemId));
        }


        public void UpdateListName(Guid listId, string name)
        {
            sampleList.Where(l => l.ListId.Equals(listId)).FirstOrDefault().Name = name;
        }


        public void UpdateList(UserList list)
        {
            var currentlist = sampleList.Where(l => l.ListId.Equals(list.ListId)).FirstOrDefault();
            currentlist = list;
        }

    }
}
