using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data.Entity;

namespace TwitterInterface.Data
{
    public class BasicEntitiesGroup
    {
        public HashTag[] Hashtags;
        public URL[] URLs;
        public UserMention[] UserMentions;
        public Symbol[] Symbols;
    }
}
