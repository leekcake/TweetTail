using Library.Container.Blend;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Library.Manager
{
    /// <summary>
    /// Provide Multi-account blending
    /// </summary>
    public class BlendManager
    {
        internal TweetTail owner;
        private string savePath;

        private List<BlendedAccount> blendeds = new List<BlendedAccount>();
        public ReadOnlyCollection<BlendedAccount> readOnlyBlendedAccounts => blendeds.AsReadOnly();
        public BlendedAccount SelectedBlendedAccount {
            get {
                if (selectedBlendName == null) return null;
                return blendeds.Find((data) => { return data.name == selectedBlendName; });
            }
        }
        public string SelectedBlendName {
            get {
                return selectedBlendName;
            }
            set {
                selectedBlendName = value;
                save();
            }
        }
        private string selectedBlendName;
        
        public BlendManager(TweetTail owner)
        {
            this.owner = owner;
            savePath = Path.Combine(owner.saveDir, "blends.json");
            load();
        }

        public void registerBlendedAccount(BlendedAccount blendedAccount)
        {
            blendeds.Add(blendedAccount);
            save();
        }

        public BlendedAccount GetBlendedAccount(string name)
        {
            return blendeds.Find((data) => { return data.name == name; });
        }

        public void unregisterBlendedAccount(BlendedAccount blendedAccount)
        {
            blendeds.RemoveAll((data) => { return data.name == blendedAccount.name; });
            save();
        }

        private void load()
        {
            if( !File.Exists(savePath) )
            {
                return;
            }

            var json = JObject.Parse( File.ReadAllText(savePath) );
            selectedBlendName = json["selectedBlendName"].ToString();

            foreach(var blendJson in json["blendeds"].ToObject<JArray>())
            {
                blendeds.Add(BlendedAccount.load(owner, blendJson.ToObject<JObject>()));
            }
        }

        public void save()
        {
            var json = new JObject();
            json["selectedBlendName"] = selectedBlendName;

            var blendArray = new JArray();
            
            foreach(var blended in blendeds)
            {
                blendArray.Add(blended.save());
            }
            json["blendeds"] = blendArray;

            File.WriteAllText(savePath, json.ToString());
        }
    }
}
