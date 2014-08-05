using KeithLink.Svc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository
{
    public class MockListRepositoryImpl: IListRepository
    {
        public static List<UserList> sampleList = new List<UserList>()
                {
                    new UserList() {
                        Name = "Sample List A",
                        ListId = Guid.NewGuid(),
                        Items = new List<ListItem>() {
                            new ListItem() {
                                ListItemId = Guid.NewGuid(),
                                ProductId = "284569",
                                Label = "This is a label",
                                Position = 0
                            },
                            new ListItem() {
                                ListItemId = Guid.NewGuid(),
                                ProductId = "287100",
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
                                ProductId = "287302",
                                Position = 0
                            },
                            new ListItem() {
                                ListItemId = Guid.NewGuid(),
                                ProductId = "287770",
                                Label = "Test Label",
                                Position = 1
                            },
                            new ListItem() {
                                ListItemId = Guid.NewGuid(),
                                ProductId = "287402",
                                Position = 2
                            }

                        }
                    }
                };

        public List<UserList> ReadAllLists()
        {
            return sampleList.Select(l => new UserList() { ListId = l.ListId, Name = l.Name }).ToList();
        }


        public UserList ReadList(Guid listId)
        {
            return sampleList.Where(l => l.ListId.Equals(listId)).FirstOrDefault();
        }


        public List<string> ReadListLabels(Guid listId)
        {
            return sampleList.Where(s => s.ListId.Equals(listId)).SelectMany(l => l.Items.Where(b => b.Label != null).Select(i => i.Label)).ToList();
        }

        public List<string> ReadListLabels()
        {
            return sampleList.SelectMany(l => l.Items.Where(b => b.Label != null).Select(i => i.Label)).ToList();
        }

        public Guid CreateList(UserList list)
        {
            list.ListId = Guid.NewGuid();
            sampleList.Add(list);
            return list.ListId;
        }


        public Guid? AddItem(Guid listId, ListItem newItem)
        {
            //TODO: Move logic out of repo and into logic class
            var list = sampleList.Where(l => l.ListId.Equals(listId)).FirstOrDefault();

            if (list == null)
                return null;

            var itemId = Guid.NewGuid();
            newItem.ListItemId = itemId;
            list.Items.Add(newItem);

            return itemId;

        }


        public void UpdateItem(Guid listId, ListItem updatedItem)
        {
            //TODO: Move logic out of repo and into logic class
            var list = sampleList.Where(l => l.ListId.Equals(listId)).FirstOrDefault();

            if (list == null)
                return;

            var item = list.Items.Where(i => i.ListItemId.Equals(updatedItem.ListItemId)).FirstOrDefault();

            item.Label = updatedItem.Label;
            item.ParLevel = updatedItem.ParLevel;
            item.Position = updatedItem.Position;
            item.ProductId = updatedItem.ProductId;

        }

        public void DeleteList(Guid listId)
        {
            sampleList.RemoveAll(l => l.ListId.Equals(listId));
        }


        public void DeleteItem(Guid listId, Guid itemId)
        {
            //TODO: Move logic out of repo and into logic class
            var list = sampleList.Where(l => l.ListId.Equals(listId)).FirstOrDefault();

            if (list == null)
                return;

            list.Items.RemoveAll(i => i.ListItemId.Equals(itemId));
        }


        public void UpdateListName(Guid listId, string name)
        {
            sampleList.Where(l => l.ListId.Equals(listId)).FirstOrDefault().Name = name;
        }
    }
}
