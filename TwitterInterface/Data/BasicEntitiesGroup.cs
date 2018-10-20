using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data.Entity;

namespace TwitterInterface.Data
{
    public class BasicEntitiesGroup
    {
        public HashTag[] hashtags;
        public URL[] urls;
        public UserMention[] userMentions;
        public Symbol[] symbols;
    }
}
