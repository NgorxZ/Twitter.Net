// ------------------------------------------------------------------------------------------------------
// Copyright (c) 2012, Kevin Wang
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the 
// following conditions are met:
//
//  * Redistributions of source code must retain the above copyright notice, this list of conditions and 
//    the following disclaimer.
//  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ------------------------------------------------------------------------------------------------------

namespace Mirai.Twitter.Commands
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mirai.Net.OAuth;
    using Mirai.Twitter.Core;
    using Mirai.Twitter.TwitterObjects;

    using fastJSON;

    public sealed class DirectMessageCommand : TwitterCommandBase
    {
        internal DirectMessageCommand(TwitterApi twitterApi)
            : base(twitterApi, "direct_messages")
        {
        }

        #region Public Methods

        public TwitterDirectMessage Destroy(string id, bool includeEntities = true)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentException();

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var postData = new Dictionary<string, string> 
                { 
                    { "include_entities", includeEntities ? "true" : "false" }
                };

            var uri         = new Uri(this.CommandBaseUri + String.Format("/destroy/{0}.json", id));
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var dm          = TwitterDirectMessage.FromDictionary(jsonObj);

            return dm;
        }

        public TwitterDirectMessage New(string screenName, string text, string userId = null, bool wrapLinks = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            if (String.IsNullOrEmpty(text))
                throw new ArgumentException();

            var postData = new Dictionary<string, string>
                {
                    { "text", text },
                    { "wrap_links", wrapLinks ? "true" : "false" }
                };

            if (screenName != null)
            {
                postData.Add("screen_name", screenName);
            }

            if (userId != null)
            {
                postData.Add("user_id", userId);
            }

            if (postData.Count == 2)
                throw new ArgumentException("Either the userIds or screenNames is required for this method.");


            var uri         = new Uri(this.CommandBaseUri + "/new.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Post, postData);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var dm          = TwitterDirectMessage.FromDictionary(jsonObj);

            return dm;
        }

        public TwitterDirectMessage[] RetrieveDirectMessages(int count = 20, string sinceId = null, 
                                                             string maxId = null, int page = 1, 
                                                             bool includeEntities = true, bool skipStatus = false)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?count={0}&page={1}&include_entities={2}&skip_status={3}",
                                      count <= 200 ? count : 200,
                                      page > 1 ? page : 1,
                                      includeEntities ? "true" : "false",
                                      skipStatus ? "true" : "false");

            if (!String.IsNullOrEmpty(sinceId))
                queryBuilder.AppendFormat("sincd_id={0}&", sinceId);
            if (!String.IsNullOrEmpty(maxId))
                queryBuilder.AppendFormat("max_id={0}", maxId);

            var uri         = new Uri(this.CommandBaseUri + ".json" + queryBuilder);
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

            var jsonArray       = (ArrayList)JSON.Instance.Parse(response);
            var directMessages  = (from Dictionary<string, object> search in jsonArray
                                   select TwitterDirectMessage.FromDictionary(search)).ToArray();

            return directMessages;
        }

        public TwitterDirectMessage[] Sent(int count = 20, string sinceId = null,
                                                             string maxId = null, int page = 1,
                                                             bool includeEntities = true)
        {
            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("?count={0}&page={1}&include_entities={2}",
                                      count <= 200 ? count : 200,
                                      page > 1 ? page : 1,
                                      includeEntities ? "true" : "false");

            if (!String.IsNullOrEmpty(sinceId))
                queryBuilder.AppendFormat("sincd_id={0}&", sinceId);
            if (!String.IsNullOrEmpty(maxId))
                queryBuilder.AppendFormat("max_id={0}", maxId);

            var uri         = new Uri(this.CommandBaseUri + "/sent.json");
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

            var jsonArray       = (ArrayList)JSON.Instance.Parse(response);
            var directMessages  = (from Dictionary<string, object> search in jsonArray
                                   select TwitterDirectMessage.FromDictionary(search)).ToArray();

            return directMessages;
        }

        public TwitterDirectMessage Show(string id)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentException();

            if (!this.TwitterApi.Authenticated)
                throw new InvalidOperationException("Authentication required.");

            var uri         = new Uri(this.CommandBaseUri + String.Format("/show/{0}.json", id));
            var response    = this.TwitterApi.ExecuteAuthenticatedRequest(uri, HttpMethod.Get, null);

            var jsonObj     = (Dictionary<string, object>)JSON.Instance.Parse(response);
            var dm          = TwitterDirectMessage.FromDictionary(jsonObj);

            return dm;
        }

        #endregion
    }
}