using System;
using System.Collections.Generic;


namespace ExceptionHandler
{

    public class ExceptionMessage
    {
        private bool _status = true;
        public object PrimaryKey { get; set; }
        public bool Status
        {
            get { return _status; }
        }

        private UserIdentity _userIdentity;

        public void Fail()
        {
            _status = false;
        }

        private List<string> _messages = new List<string>();
        public List<string> Messages
        {
            get { return _messages; }
            private set { _messages.AddRange(value); }
        }

        private List<string> _user_messages = new List<string>();
        public List<string> UserMessages
        {
            get { return _user_messages; }
        }

        public string LastMessage
        {
            get
            {
                if (_messages.Count == 0)
                    return "";
                return _messages[_messages.Count - 1];
            }
            set { _messages.Add(value + " Time : " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")); }
        }
        public string LastUserMessage
        {
            get
            {
                if (_user_messages.Count == 0)
                    return "No User Messages Set";
                return _user_messages[_user_messages.Count - 1];
            }
            set { _user_messages.Add(value); }
        }

        public UserIdentity UserIdentity { get => _userIdentity; set => _userIdentity = value; }

        public void MergeMessages(ExceptionMessage message)
        {
            this._messages.AddRange(message._messages);
            this._user_messages.AddRange(message._user_messages);
            if (!message._status)
                this._status = false;
        }

        public ExceptionMessage(string message, string userMessage, bool status)
        {
            this._messages.Add(message);
            this._user_messages.Add(userMessage);
            this._status = status;
            _userIdentity = new UserIdentity();
        }

        public ExceptionMessage()
        {
            this._messages.Add("No Message");
            _userIdentity = new UserIdentity();
        }

        public ExceptionMessage(string message)
        {
            this._messages.Add(message);
            _userIdentity = new UserIdentity();
        }
    }
    public class UserIdentity
    {
        private string _userId = "";
        private string _userPassword = "";
        private string _domain = "";

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string UserPassword
        {
            get { return _userPassword; }
            set { _userPassword = value; }
        }

        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }
    }
}

