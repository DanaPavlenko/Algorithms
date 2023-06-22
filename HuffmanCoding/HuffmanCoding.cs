/*
 * @file   HuffmanCoding.cs
 * @author Dana
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace HuffmanCoding
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1) {

                if (args[0] == "encode") {
                    HuffmanTree huffmanTree = new HuffmanTree();
                    huffmanTree.Encode(args[1]);
                }
                else if (args[0] == "decode") {
                    HuffmanTree huffmanTree = new HuffmanTree();
                    huffmanTree.Decode(args[1]);
                }
                else {
                    Console.WriteLine("ERROR: No operation type");
                }
            }
            else {
                Console.WriteLine("ERROR: sintax");
            }
        }

        [Serializable]
        public class Node
        {
            public char Symbol { get; set; }
            public int Frequency { get; set; }
            public Node Right { get; set; }
            public Node Left { get; set; }

            public List<bool> Traverse(char symbol, List<bool> data)
            {
                // Leaf
                if (Right == null && Left == null) {
                    if (symbol.Equals(this.Symbol)) {
                        return data;
                    }
                    else {
                        return null;
                    }
                }
                else {
                    List<bool> left = null;
                    List<bool> right = null;

                    if (Left != null) {
                        List<bool> leftPath = new List<bool>();
                        leftPath.AddRange(data);
                        leftPath.Add(false);

                        left = Left.Traverse(symbol, leftPath);
                    }

                    if (Right != null) {
                        List<bool> rightPath = new List<bool>();
                        rightPath.AddRange(data);
                        rightPath.Add(true);
                        right = Right.Traverse(symbol, rightPath);
                    }

                    if (left != null) {
                        return left;
                    }
                    else {
                        return right;
                    }
                }
            }
        }

        public class HuffmanTree
        {
            private List<Node> nodes = new List<Node>();
            public Node Root { get; set; }
            public Dictionary<char, int> Frequencies = new Dictionary<char, int>();

            public void Build(string source)
            {
                for (int i = 0; i < source.Length; i++) {
                    if (!Frequencies.ContainsKey(source[i])) {
                        Frequencies.Add(source[i], 0);
                    }
                    Frequencies[source[i]]++;
                }

                foreach (KeyValuePair<char, int> symbol in Frequencies) {
                    nodes.Add(new Node() { Symbol = symbol.Key, Frequency = symbol.Value });
                }

                while (nodes.Count > 1) {
                    List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<Node>();

                    if (orderedNodes.Count >= 2) {
                        // take first two items
                        List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                        // create a parent node by combining the frequencies
                        Node parent = new Node() {
                            Symbol = '*',
                            Frequency = taken[0].Frequency + taken[1].Frequency,
                            Left = taken[0],
                            Right = taken[1]
                        };

                        nodes.Remove(taken[0]);
                        nodes.Remove(taken[1]);
                        nodes.Add(parent);
                    }

                    this.Root = nodes.FirstOrDefault();
                }
            }

            public void Encode(string infile)
            {
                string outfile = string.Empty;
                if (infile.Contains('\\')) {
                    if (infile.Substring(infile.LastIndexOf('\\')).Contains('.')) {
                        outfile = infile.Substring(0, infile.LastIndexOf('.')) + "_enc" +
                            infile.Substring(infile.LastIndexOf('.'));
                    }
                    else {
                        outfile = infile + "_enc";
                    }
                }
                else {
                    if (infile.Contains('.')) {
                        outfile = infile.Substring(0, infile.LastIndexOf('.')) + "_enc" +
                            infile.Substring(infile.LastIndexOf('.'));
                    }
                    else {
                        outfile = infile + "_enc";
                    }
                }
                FileStream outstream = new FileStream(outfile, FileMode.Create, FileAccess.Write, FileShare.None);
                //StreamWriter outstream = new StreamWriter(outfile, false, Encoding.UTF8);
              
                List<bool> encodedSource = new List<bool>();

                StreamReader instream = new StreamReader(infile, Encoding.Default);
                string source = instream.ReadToEnd();

                this.Build(source);

                for (int i = 0; i < source.Length; i++) {
                    List<bool> encodedSymbol = this.Root.Traverse(source[i], new List<bool>());
                    encodedSource.AddRange(encodedSymbol);
                }

                BitArray bits = new BitArray(encodedSource.ToArray());

                Byte[] e = new Byte[(bits.Length / 8 + (bits.Length % 8 == 0 ? 0 : 1))];
                bits.CopyTo(e, 0);

                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(outstream, Root);
                
                outstream.Write(e, 0, e.Length);
                outstream.Close();
            }

            public void Decode(string infile)
            {
                string outfile;
                if (infile.Contains('\\')) {
                    if (infile.Substring(infile.LastIndexOf('\\')).Contains('.')) {
                        if (infile.Contains("_enc")) {
                            outfile = infile.Substring(0, infile.LastIndexOf('_')) + "_dec" +
                                infile.Substring(infile.LastIndexOf('.'));
                        }
                        else {
                            outfile = infile.Substring(0, infile.LastIndexOf('.')) + "_dec" +
                                infile.Substring(infile.LastIndexOf('.'));
                        }
                    }
                    else {
                        if (infile.Contains("_enc")) {
                            outfile = infile.Substring(0, infile.LastIndexOf('_')) + "_dec";
                        }
                        else {
                            outfile = infile + "_dec";
                        }
                    }
                }
                else {
                    if (infile.Contains('.')) {
                        if (infile.Contains("_enc")) {
                            outfile = infile.Substring(0, infile.LastIndexOf('_')) + "_dec" +
                                infile.Substring(infile.LastIndexOf('.'));
                        }
                        else {
                            outfile = infile.Substring(0, infile.LastIndexOf('.')) + "_dec" +
                                infile.Substring(infile.LastIndexOf('.'));
                        }
                    }
                    else {
                        if (infile.Contains("_enc")) {
                            outfile = infile.Substring(0, infile.LastIndexOf('_')) + "_dec";
                        }
                        else {
                            outfile = infile + "_dec";
                        }
                    }
                }

                FileStream instream = new FileStream(infile, FileMode.Open, FileAccess.Read, FileShare.Read);
                StreamWriter outstream = new StreamWriter(outfile, false, Encoding.Default);

                BinaryFormatter formatter = new BinaryFormatter();
                Node Root = (Node)formatter.Deserialize(instream);

                byte[] source = new byte[instream.Length - instream.Position];
                instream.Read(source, 0, (int)(instream.Length - instream.Position));
                instream.Close();
                
                BitArray bits = new BitArray(source);
                Node current = Root;
                string decoded = "";

                foreach (bool bit in bits) {
                    if (bit) {
                        if (current.Right != null) {
                            current = current.Right;
                        }
                    }
                    else {
                        if (current.Left != null) {
                            current = current.Left;
                        }
                    }

                    if (IsLeaf(current)) {
                        decoded += current.Symbol;
                        current = Root;
                    }
                }
                outstream.Write(decoded);
                outstream.Close();
            }

            public bool IsLeaf(Node node)
            {
                return (node.Left == null && node.Right == null);
            }

        }
    }
}