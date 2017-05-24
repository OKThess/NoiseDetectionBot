﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SampleAADV2Bot.Helpers
{
    public class MeetingRoom
    {
        public string DisplayName { get; set; }
        public string LocationEmailAddress { get; set; }
    }

    public class UserInfo
    {
        public string DisplayName { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string JobTitle { get; set; }
        public string Mail { get; set; }

    }
    public class GraphHelper
    {
        public string Token { get; set; }

        public async Task<UserInfo > GetUserInfo()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + this.Token);

                var userresponse = await client.GetAsync("https://graph.microsoft.com/beta/me/");
                dynamic userInfo = JObject.Parse(await userresponse.Content.ReadAsStringAsync());

                return new UserInfo()
                {
                    DisplayName = userInfo.displayName,
                    Firstname = userInfo.givenName,
                    Mail = userInfo.mail,
                    JobTitle = userInfo.jobTitle
                };


            }
            catch (Exception)
            {
                throw;
            }           
        }

        public async Task<List<MeetingRoom>> GetMeetingRoomSuggestions()
        {
            try
            {

                List<MeetingRoom> suggestions = new List<MeetingRoom>();
                HttpClient client = new HttpClient();

                var meetingresponse = await client.PostAsync("https://graph.microsoft.com/beta/me/findMeetingTimes", new StringContent(String.Empty));

                dynamic meetingTimes = JObject.Parse(await meetingresponse.Content.ReadAsStringAsync());

                foreach (var item in meetingTimes.meetingTimeSuggestions[0].locations)
                {
                    // Add only locations with an email address -> meeting rooms
                    if (!String.IsNullOrEmpty(item.locationEmailAddress.ToString()))
                        suggestions.Add(new MeetingRoom()
                        {
                            DisplayName = item.displayName,
                            LocationEmailAddress = item.locationEmailAddress
                        });
           
                }

                return suggestions;
            }
            catch (Exception)
            {

                throw;
            }
        
        
        }

    }
}