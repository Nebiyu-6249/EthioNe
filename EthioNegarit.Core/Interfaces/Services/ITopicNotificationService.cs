using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;


namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface ITopicNotificationService
    {
      IList<TopicNotification> GetAll();
        void Delete(TopicNotification notification);
        IList<TopicNotification> GetByTopic(Topic topic);
        IList<TopicNotification> GetByUserAndTopic(MembershipUser user, Topic topic, bool addTracking = false);
        TopicNotification Add(TopicNotification topicNotification);
    }
}
