using EthioNegarit.Core.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
namespace EthioNegarit.Core.Domain
{
    public class RecentActivity
    {
        private List<RecentVisited> topicCollection = new List<RecentVisited>();
        public void AddRecentTopicVisited(Topic topic)
        {
            RecentVisited recentTopic = topicCollection.Where(p => p.Topic.Id == topic.Id).FirstOrDefault();
            if (recentTopic == null)
            {
                topicCollection.Add(new RecentVisited
                {
                    Topic = topic
                });
            }
        }
        public IEnumerable<RecentVisited> RecentTopics
        {
            get { return topicCollection; }
        }
    
        public void RemoveTopic(Topic topic)
        {
            topicCollection.RemoveAll(l => l.Topic.Id == topic.Id);
        }

        public void Clear()
        {
            topicCollection.Clear();
        }
    }
    public class RecentVisited
    {
        public Topic Topic { get; set; }
    }
}
