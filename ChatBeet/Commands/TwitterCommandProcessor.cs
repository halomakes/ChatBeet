﻿using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot.Commands;
using GravyIrc.Messages;
using LinqToTwitter;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class TwitterCommandProcessor : CommandProcessor
    {
        private readonly TwitterService tweetService;

        public TwitterCommandProcessor(TwitterService tweetService)
        {
            this.tweetService = tweetService;
        }

        [Command("tweet {username}", Description = "Get the latest tweet from a user.")]
        public async Task<IClientMessage> GetRecentTweet(
            [Required, RegularExpression(@"^@?[A-Za-z0-9_]{1,15}$", ErrorMessage = "Enter a valid Twitter handle.")] string username
            )
        {
            try
            {
                var tweet = await tweetService.GetRecentTweet(username, false, false);

                if (tweet == default)
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), "Sorry, couldn't find anything recent.");
                }
                else
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), tweet.ToIrcMessage());
                }
            }
            catch (TwitterQueryException)
            {
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), "Sorry, couldn't find that account.");
            }
        }

        [Command("miata", Description = "Get a random #miata from Twitter.")]
        public async Task<IClientMessage> GetMiata()
        {
            try
            {
                var tweet = await tweetService.GetRandomTweetByHashtag("miata", true, filter: s => s.FullText.ToLower().Contains("miata"));

                if (tweet == default)
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), "Sorry, couldn't find anything recent.");
                }
                else
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), tweet.ToIrcMessage());
                }
            }
            catch (TwitterQueryException)
            {
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), "Sorry, couldn't find anything.");
            }
        }
    }
}
