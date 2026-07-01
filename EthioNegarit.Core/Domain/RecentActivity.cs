using EthioNegarit.Core.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace EthioNegarit.Core.Domain
    {
    public class RecentActivity
    {
        private List<RecentVisited> topicCollection = new List<RecentVisited>();
        private List<RecentVisited> categoryCollection = new List<RecentVisited>();
        private List<RecentVisited> chatRoomCollection = new List<RecentVisited>();
        public void AddRecentTopicVisited(Topic topic)
          {
              RecentVisited recentTopic = topicCollection.Where(p => p.Topic.Id == topic.Id).FirstOrDefault();
                        
              if (recentTopic == null)
              {
                  topicCollection.Add(new RecentVisited
                  {
                      Topic = topic,
                 
                  });
              }            
          }
        public void AddRecentCategoryVisited(Category category)
        {
            RecentVisited recentCategory = categoryCollection.Where(c => c.Category.Id == category.Id).FirstOrDefault();
            if (recentCategory == null)
            {
                categoryCollection.Add(new RecentVisited
                {
                    Category = category,

                });
            }

        }
        public void AddRecentChatroomVisited(ChatRoom chatRoom)
        {
            RecentVisited recentRoom = chatRoomCollection.Where(c => c.ChatRoom.Id == chatRoom.Id).FirstOrDefault();
            if (recentRoom == null)
            {
                chatRoomCollection.Add(new RecentVisited
                {
                    ChatRoom = chatRoom,

                });
            }
        }
        public IEnumerable<RecentVisited> RecentTopics
        {
            get { return topicCollection; }
        }
        public IEnumerable<RecentVisited> RecentCategories
        {
            get { return categoryCollection; }
        }
        public IEnumerable<RecentVisited> RecentChatRoom
        {
            get { return chatRoomCollection; }
        }
        // Used by Topic/ClearVisited to reset the visitor's recent-activity history.
        public void Clear()
        {
            topicCollection.Clear();
            categoryCollection.Clear();
            chatRoomCollection.Clear();
        }
    }
    public class RecentVisited
    {
        public Topic Topic { get; set; }
        public Category Category { get; set; }
        public ChatRoom ChatRoom { get; set; }
    }
}
