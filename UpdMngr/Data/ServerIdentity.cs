﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using UpdMngr.Api;

namespace UpdMngr.Data
{
    [XmlRoot("srvid")]
    public class ServerIdentity : IServerIdentity
    {
        [XmlIgnore()]
        public IReadOnlyAuthorizationCookie AuthorizationCookie
        {
            get { return _AuthorizationCookie; }
            set { _AuthorizationCookie = (AuthCookie)value; }
        }

        [XmlElement("authorizationCookie", Type = typeof(AuthCookie), IsNullable = true)]
        public AuthCookie _AuthorizationCookie { get; set; }

        [XmlIgnore()]
        internal XmlBasedPersistenceProvider Owner
        {
            get { return _owner; }
            set
            {
                if (null == value) { throw new ArgumentNullException(); }
                if (null != _owner) {
                    throw new InvalidOperationException();
                }
                _owner = value;
            }
        }

        [XmlAttribute("id")]
        public string ServerId { get; set; }

        [XmlAttribute("name")]
        public string ServerName { get; set; }

        [XmlIgnore()]
        public IReadOnlyCookie UpstreamServerCookie
        {
            get { return _UpstreamServerCookie; }
            set { _UpstreamServerCookie = (Cookie)value; }
        }

        [XmlElement("upstreamCookie", IsNullable = true)]
        public Cookie _UpstreamServerCookie { get; set; }

        public void RegisterAuthorizationCookieData(string pluginId, byte[] data)
        {
            AuthorizationCookie = new AuthCookie(pluginId, data);
            _owner.RewriteIdentity(this);
            return;
        }

        public void RegisterUpstreamCookieData(DateTime expiracy, byte[] data)
        {
            UpstreamServerCookie = new Cookie(expiracy, data);
            _owner.RewriteIdentity(this);
            return;
        }

        private XmlBasedPersistenceProvider _owner;

        public class AuthCookie : IReadOnlyAuthorizationCookie
        {
            public AuthCookie()
            {
            }

            internal AuthCookie(string id, byte[] data)
            {
                if (string.IsNullOrEmpty(id)) { throw new ArgumentNullException(); }
                if ((null == data) || (0 == data.Length)) { throw new ArgumentNullException(); }
                PlugInId = id;
                CookieData = data;
            }

            [XmlAttribute("data", DataType = "hexBinary")]
            public byte[] CookieData { get; set; }

            [XmlAttribute("pluginId")]
            public string PlugInId { get; set; }
        }

        public class Cookie : IReadOnlyCookie
        {
            public Cookie()
            {
            }

            internal Cookie(DateTime expiracy, byte[] data)
            {
                EncryptedData = data;
                Expiration = expiracy;
            }

            [XmlAttribute("data", DataType = "hexBinary")]
            public byte[] EncryptedData { get; set; }

            [XmlAttribute("expiration")]
            public DateTime Expiration { get; set; }
        }
    }
}
