using CrossX.Forms.Helpers;
using CrossX.Forms.Xml;
using CrossX.IO;
using CrossX.IoC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CrossX.Forms.Transitions
{
    internal class TransitionsManager : ITransitionsManager
    {
        private readonly IFilesRepository filesRepository;
        private readonly IObjectFactory objectFactory;
        private Dictionary<string, StateTransition> stateTransitions = new Dictionary<string, StateTransition>();
        private Dictionary<string, Transition> transitions = new Dictionary<string, Transition>();

        public TransitionsManager(IFilesRepository filesRepository, IObjectFactory objectFactory)
        {
            this.filesRepository = filesRepository;
            this.objectFactory = objectFactory;
        }

        public StateTransition CreateStateTransition(string key, string name)
        {
            return stateTransitions[key].Clone(name);
        }

        public Transition CreateTransition(string key, string name)
        {
            return transitions[key].Clone(name);
        }

        public void LoadTransitions(string path)
        {
            XNode node;
            using (var stream = filesRepository.Open(path))
            {
                var reader = XmlReader.Create(stream);
                node = XNode.ReadXml(reader);
            }

            if (node.Tag != "Transitions") throw new InvalidDataException();

            foreach (var cn in node.Nodes)
            {
                if (cn.Tag == "Transition")
                {
                    ParseTransition(cn);
                }
                else if(cn.Tag == "StateTransition")
                {
                    ParseStateTransition(cn);
                }
            }
        }

        private void ParseTransition(XNode node)
        {
            
        }

        private void ParseStateTransition(XNode node)
        {
            var key = node.Attribute("Key");
            var attr = new XNodeAttributes(node);

            var duration = (float)attr.AsInt32("Duration", 100) / 1000.0f;
            var inverted = attr.AsBoolean("Inverted");

            var transforms = new Transform[node.Nodes.Count];

            for(var idx =0; idx < node.Nodes.Count; ++idx)
            {
                transforms[idx] = ParseTransform(node.Nodes[idx]);
            }

            stateTransitions.Add(key, new StateTransition(key, duration, inverted, transforms));
        }

        private Transform ParseTransform(XNode node)
        {
            var type = XmlHelpers.TypeFromNode(node);
            var attr = new XNodeAttributes(node);

            return (Transform)objectFactory.Create(type, attr);
        }
    }
}
