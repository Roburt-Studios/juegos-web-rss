using System.Collections.Generic;

public class Dialog
{
    public class Option
    {
        public string text;
        public string nextId;
        public string failureNextId;
        public int difficulty;

        public Option(string text, string nextId, string failureNextId, int difficulty)
        {
            this.text = text;
            this.nextId = nextId;
            this.failureNextId = failureNextId;
            this.difficulty = difficulty;
        }
    }

    public class Node
    {
        public string id;
        public string characterId;
        public string text;
        public string nextId;
        public bool isChoice;
        public string tag;
        public readonly List<Option> options = new();

        public Node(string id, string characterId, string text, string nextId, bool isChoice, string tag)
        {
            this.id = id;
            this.characterId = characterId;
            this.text = text;
            this.nextId = nextId;
            this.isChoice = isChoice;
            this.tag = tag;
        }
    }

    public readonly string id;
    public string startNodeId { get; private set; }
    private readonly Dictionary<string, Node> nodes = new();

    public Dialog(string id) => this.id = id;

    public bool AddNode(Node node)
    {
        if (nodes.ContainsKey(node.id))
            return false;

        if (startNodeId == null)
            startNodeId = node.id;

        nodes.Add(node.id, node);
        return true;
    }

    public bool TryGetNode(string nodeId, out Node node) => nodes.TryGetValue(nodeId, out node);
}
