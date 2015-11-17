// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/RedditAPI.cs" was last cleaned by Rick on 2015/06/12 at 2:56 PM

namespace Librainian.Internet.Reddit {

    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using CsQuery.Utility;

    internal class RedditApi {
        private readonly String _cookiefn;
        private readonly WebClient _jsonGet;
        private readonly CookieContainer _redditCookie;

        //A cache of me.json, since we likely don't need to retrieve it every time we use it

        private readonly String _usr;
        private Hashtable _me = new Hashtable();

        /// <summary>
        /// Class constructor, logs in or checks for previous login from cookie file
        /// </summary>
        /// <param name="user">Reddit account username</param>
        /// <param name="pswd">Reddit Account password</param>
        /// <param name="cookiefilename">File name of the cookie</param>
        public RedditApi(String user, String pswd, String cookiefilename = "cookie") {
            this._redditCookie = new CookieContainer();
            this._jsonGet = new WebClient();
            this._usr = user;
            this._cookiefn = cookiefilename;
            this._redditCookie = Loadcookie( cookiefilename );
            var tmpc = this._redditCookie.GetCookies( new Uri( "http://www.reddit.com/api/login/" + this._usr ) );
            if ( this._redditCookie == null ) {
                this.Login( user, pswd );
            }

            this.GetMe();
        }

        /// <summary>Downvotes a "thing"</summary>
        /// <param name="postID">"thing" ID</param>
        /// <returns></returns>
        /// <remarks>See Glossary here for more info on "things" https://github.com/reddit/reddit/wiki/API</remarks>
        public Boolean Downvote(String postID) {
            this.Vote( postID, -1 );
            return true;
        }

        /// <summary>Returns me.json's data as a hashtable</summary>
        /// <returns>me.json's data as a hashtable</returns>
        /// <remarks>e.g. (String)me["modhash"] would be the user's modhash as a String</remarks>
        public Hashtable GetMeCache() => this._me;

        /// <summary>Checks if the user has mail based on a fresh polling of me.json</summary>
        /// <returns>True/false depending on if the user has an orangered</returns>
        public Boolean HasMail() {

            //Get a fresh copy of me.json
            this.GetMe();
            return ( String )this._me[ "has_mail" ] == "true";
        }

        /// <summary>Posts a comment to the specified "thing"</summary>
        /// <param name="id">"thing" ID code</param>
        /// <param name="content">Comment contents</param>
        /// <returns>True/false based on success (NYI)</returns>
        /// <remarks>See Glossary here for more info on "things" https://github.com/reddit/reddit/wiki/API</remarks>
        public Boolean PostComment(String id, String content) {
            var modhash = ( String )this._me[ "modhash" ];
            this.SendPost( $"thing_id={id}&text={content}&uh={modhash}", "http://www.reddit.com/api/comment" );
            return true;
        }

        /// <summary>Posts a link</summary>
        /// <param name="link">URI of post</param>
        /// <param name="title">Title of submission</param>
        /// <param name="sr">Subreddit to post to</param>
        public void PostLink(String link, String title, String sr) => this.Post( "self", link, sr, title );

        /// <summary>Posts a self post</summary>
        /// <param name="link">Text of the self post</param>
        /// <param name="title">Title of submission</param>
        /// <param name="sr">Subreddit to post to</param>
        public void PostSelf(String link, String title, String sr) => this.Post( "self", link, sr, title );

        /// <summary>Rescinds vote from "thing"</summary>
        /// <param name="postID">"thing" ID</param>
        /// <returns></returns>
        /// <remarks>See Glossary here for more info on "things" https://github.com/reddit/reddit/wiki/API</remarks>
        public Boolean UnVote(String postID) {
            this.Vote( postID, 0 );
            return true;
        }

        /// <summary>Upvotes a "thing"</summary>
        /// <param name="postID">"thing" ID</param>
        /// <returns></returns>
        /// <remarks>See Glossary here for more info on "things" https://github.com/reddit/reddit/wiki/API</remarks>
        public Boolean Upvote(String postID) {
            this.Vote( postID, 1 );
            return true;
        }

        /// <summary>Loads cookie from file</summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static CookieContainer Loadcookie(String filename) {
            Stream stream = File.Open( filename, FileMode.Open );
            var bFormatter = new BinaryFormatter();
            var rcookie = ( CookieContainer )bFormatter.Deserialize( stream );
            stream.Close();
            return rcookie;
        }

        /// <summary>Saves cookie to file</summary>
        /// <param name="filename"></param>
        /// <param name="rcookie"></param>
        private static void Savecookie(String filename, CookieContainer rcookie) {
            Stream stream = File.Open( filename, FileMode.Create );
            var bFormatter = new BinaryFormatter();
            bFormatter.Serialize( stream, rcookie );
            stream.Close();
        }

        /// <summary>Gets a fresh copy of me.json and saves it to the cache</summary>
        /// <returns>True/false depending on success (NYI)</returns>
        private Hashtable GetMe() {
            this._jsonGet.Headers[ "COOKIE" ] = this._redditCookie.GetCookieHeader( new Uri( "http://www.reddit.com/api/login/" + this._usr ) );
            var jsonStream = this._jsonGet.OpenRead( "http://www.reddit.com/api/me.json" );
            var jSr = new StreamReader( jsonStream );
            var metmp = jSr.ReadToEnd();
            var meData = JSON.ParseJSON( metmp ) as Hashtable; //TODO untested

                                                               // ReSharper disable once PossibleNullReferenceException
            this._me = ( Hashtable )meData[ "data" ];

            return this._me;
        }

        /// <summary>Logs the user in</summary>
        /// <param name="user">Reddit account username</param>
        /// <param name="pswd">Reddit account password</param>
        /// <returns>True/False depending on success of login (NYI)</returns>
        private Boolean Login(String user, String pswd) {
            var login = WebRequest.Create( "http://www.reddit.com/api/login/" + user ) as HttpWebRequest;
            if ( null == login ) {
                return false;
            }
            login.CookieContainer = this._redditCookie;
            login.Method = "POST";
            login.ContentType = "application/x-www-form-urlencoded";

            var postData = $"api_type=json&user={user}&passwd={pswd}";
            var dataBytes = Encoding.ASCII.GetBytes( postData );
            login.ContentLength = dataBytes.Length;
            var postStream = login.GetRequestStream();

            postStream.Write( dataBytes, 0, dataBytes.Length );
            postStream.Close();

            //Do the actual login
            var response = login.GetResponse();
            var r = new StreamReader( stream: response.GetResponseStream() );
            Console.WriteLine( r.ReadToEnd() );
            Console.WriteLine( this._redditCookie.GetCookieHeader( new Uri( "http://www.reddit.com/api/login/" + this._usr ) ) );

            Savecookie( this._cookiefn, this._redditCookie );
            return true;
        }

        /// <summary>Posts a link/self post</summary>
        /// <param name="kind">"self" or "link"</param>
        /// <param name="url">URL or Self Post content</param>
        /// <param name="sr">subreddit</param>
        /// <param name="title">Title of post</param>
        /// <returns></returns>
        private Boolean Post(String kind, String url, String sr, String title) {
            var modhash = ( String )this._me[ "modhash" ];
            this.SendPost( String.Format( "uh={0}&kind={1}&url={2}&sr={3}&title={4}&r={3}&renderstyle=html", ( String )this._me[ "modhash" ], kind, url, sr, title ), "http://www.reddit.com/api/submit" );
            return true;
        }

        /// <summary>Sends data in POST to the specified URI</summary>
        /// <param name="data">POST data</param>
        /// <param name="uri">URI to POST data to</param>
        /// <returns>True/false based on success (NYI)</returns>
        private Boolean SendPost(String data, String uri) {
            var connect = WebRequest.Create( new Uri( uri ) ) as HttpWebRequest;
            if ( null == connect ) {
                return false;
            }
            connect.Headers[ "COOKIE" ] = this._redditCookie.GetCookieHeader( new Uri( uri ) );
            connect.CookieContainer = this._redditCookie;
            connect.Method = "POST";
            connect.ContentType = "application/x-www-form-urlencoded";

            var dataBytes = Encoding.ASCII.GetBytes( data );
            connect.ContentLength = dataBytes.Length;

            var postStream = connect.GetRequestStream();
            postStream.Write( dataBytes, 0, dataBytes.Length );
            postStream.Close();

            //Do the actual connection
            connect.GetResponse();
            return true;
        }

        /// <summary>Casts a vote on the specified "thing"</summary>
        /// <param name="post">"thing" ID</param>
        /// <param name="type">Vote type, 1, 0 or -1</param>
        /// <returns>True/false based on success (NYI)</returns>
        private Boolean Vote(String post, Int32 type) {
            var modhash = ( String )this._me[ "modhash" ];
            this.SendPost( $"id={post}&dir={type}&uh={modhash}", "http://www.reddit.com/api/vote" );

            return true;
        }
    }
}