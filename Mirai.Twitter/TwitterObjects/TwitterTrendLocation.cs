﻿// ------------------------------------------------------------------------------------------------------
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

namespace Mirai.Twitter.TwitterObjects
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Mirai.Twitter.Core;

    public sealed class TwitterTrendLocation
    {
        [TwitterKey("country")]
        public string Country { get; set; }

        [TwitterKey("countryCode")]
        public string CountryCode { get; set; }

        public string LocationType
        {
            get { return this.PlaceType.Name; }
        }

        public string LocationCode
        {
            get { return this.PlaceType.Code; }
        }

        [TwitterKey("name")]
        public string Name { get; set; }

        [TwitterKey("parentid")]
        public string ParentId { get; set; }

        [TwitterKey("url")]
        public Uri Url { get; set; }

        [TwitterKey("woeid")]
        public string WoeId { get; set; }


        [TwitterKey("placeType")]
        private TwitterTrendLocationType PlaceType { get; set; }


        public static TwitterTrendLocation FromDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            var location = new TwitterTrendLocation();
            if (dictionary.Count == 0)
                return location;

            var pis = location.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                                                                                   typeof(TwitterKeyAttribute));

                object value;
                if (twitterKey == null || dictionary.TryGetValue(twitterKey.Key, out value) == false || value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(String))
                {
                    propertyInfo.SetValue(location, value, null);
                }
                else if (propertyInfo.PropertyType == typeof(Uri))
                {
                    propertyInfo.SetValue(location, new Uri(value.ToString()), null);
                }
                else if (propertyInfo.PropertyType == typeof(TwitterTrendLocationType))
                {
                    propertyInfo.SetValue(location, 
                        TwitterTrendLocationType.FromDictionary(value as Dictionary<string, object>), null);
                }
            }

            return location;
        }
    }

    internal sealed class TwitterTrendLocationType
    {
        [TwitterKey("name")]
        public string Name { get; set; }

        [TwitterKey("code")]
        public string Code { get; set; }


        public static TwitterTrendLocationType FromDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            var locationType = new TwitterTrendLocationType();
            if (dictionary.Count == 0)
                return locationType;

            var pis = locationType.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in pis)
            {
                var twitterKey = (TwitterKeyAttribute)Attribute.GetCustomAttribute(propertyInfo,
                                                                                   typeof(TwitterKeyAttribute));

                object value;
                if (twitterKey == null || dictionary.TryGetValue(twitterKey.Key, out value) == false
                    || value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(String))
                {
                    propertyInfo.SetValue(locationType, value, null);
                }
            }

            return locationType;
        }
    }
}
