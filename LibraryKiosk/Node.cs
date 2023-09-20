using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryKiosk
{
    public class Node
    {
        public Book Book { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public int Height { get; set; }

        public Node(Book book)
        {
            Book = book;
            Height = 1;
        }
    }

    public class AVLTree
    {
        public Node root;
        public delegate string BookKeySelector(Book book);
        public BookKeySelector SortFunc;
        public List<Book> AllBooks { get; private set; } = new List<Book>();

        public AVLTree(bool loadDataFromCSV = true)
        {
            SortFunc = book => book.Title; // Default to sorting by title
            if (loadDataFromCSV)
            {
                LoadBooksFromCSV();  // Load books from CSV when the AVLTree is instantiated.
            }
        }
        public void LoadBooksFromCSV()
        {
            using (var reader = new StreamReader("books.csv"))
            using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false }))
            {
                var records = csv.GetRecords<Book>();
                foreach (var book in records)
                {
                    this.InsertBook(book);
                }
            }
        }
        public void InOrderTraversal(Node node)
        {
            if (node != null)
            {
                InOrderTraversal(node.Left);
                node.Book.Print();
                InOrderTraversal(node.Right);
            }
        }
        public void ChangeSortingKey(System.Func<LibraryKiosk.Book, string> primarySortingKey, System.Func<LibraryKiosk.Book, string> secondarySortingKey = null)
        {
            var books = new List<Book>();
            StoreBooks(root, books);

            SortFunc = book =>
            {
                var primaryKey = primarySortingKey(book);
                var secondaryKey = secondarySortingKey != null ? secondarySortingKey(book) : "";

                if (string.IsNullOrEmpty(primaryKey))
                {
                    return char.MaxValue + secondaryKey;  
                }
                return primaryKey;
            };

            root = null; // clear the tree

            foreach (var book in books)
            {
                InsertBook(book);
            }
        }

        private void StoreBooks(Node node, List<Book> books)
        {
            if (node == null)
                return;

            StoreBooks(node.Left, books);
            books.Add(node.Book);
            StoreBooks(node.Right, books);
        }
        public Node Search(Node root, string key)
        {
            if (root == null || SortFunc(root.Book) == key)
                return root;

            if (SortFunc(root.Book).CompareTo(key) < 0)
                return Search(root.Right, key);

            return Search(root.Left, key);
        }



        public void InsertBook(Book book, bool updateAllBooks = true)
        {
            var key = SortFunc(book);
            if (string.IsNullOrEmpty(key))
            {
                key = char.MaxValue.ToString(); //missing values to the end
            }
            this.root = this.Insert(this.root, key, book);

            if (updateAllBooks)
            {
                AllBooks.Add(book);
            }
        }

        public Node Insert(Node node, string key, Book book)
        {
            if (node == null)
                return (new Node(book));

            if (string.Compare(key, SortFunc(node.Book)) < 0)
                node.Left = Insert(node.Left, key, book);
            else if (string.Compare(key, SortFunc(node.Book)) > 0)
                node.Right = Insert(node.Right, key, book);
            else // No duplicate keys
                return node;

            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

            int balance = GetBalance(node);


            if (balance > 1 && string.Compare(key, SortFunc(node.Left.Book)) < 0)
                return RightRotate(node);

            if (balance < -1 && string.Compare(key, SortFunc(node.Right.Book)) > 0)
                return LeftRotate(node);

            if (balance > 1 && string.Compare(SortFunc(book), SortFunc(node.Left.Book)) > 0)
            {
                if (node.Left != null)
                    node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            if (balance < -1 && string.Compare(SortFunc(book), SortFunc(node.Right.Book)) < 0)
            {
                if (node.Right != null)
                    node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        int Height(Node node)
        {
            if (node == null)
                return 0;
            return node.Height;
        }

        int GetBalance(Node node)
        {
            if (node == null)
                return 0;
            return Height(node.Left) - Height(node.Right);
        }

        Node RightRotate(Node y)
        {
            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            Node x = y.Left;

            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            Node T2 = x.Right;

            // Perform rotation
            x.Right = y;
            y.Left = T2;

            // Update heights
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

            // Return new root
            return x;
        }

        Node LeftRotate(Node x)
        {
            if (x == null || x.Right == null)
            {
                throw new ArgumentNullException(nameof(x), "Cannot rotate a null node.");
            }

            Node y = x.Right;
            Node T2 = y.Left;

            // Perform rotation
            y.Left = x;
            x.Right = T2;

            //  Update heights
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

            // Return new root
            return y;
        }

        public void DeleteBookByTitle(string title)
        {
            this.root = Delete(this.root, title);

            var bookToDelete = AllBooks.FirstOrDefault(b => b.Title == title);
            if (bookToDelete != null)
            {
                AllBooks.Remove(bookToDelete);
            }
        }

        private Node Delete(Node root, string title)
        {
            if (root == null) return root;

            string rootKey = SortFunc(root.Book);
            if (string.Compare(title, rootKey) < 0)
                root.Left = Delete(root.Left, title);
            else if (string.Compare(title, rootKey) > 0)
                root.Right = Delete(root.Right, title);
            else
            {
                if ((root.Left == null) || (root.Right == null))
                {
                    Node temp = null;
                    if (temp == root.Left)
                        temp = root.Right;
                    else
                        temp = root.Left;

                    if (temp == null)
                    {
                        temp = root;
                        root = null;
                    }
                    else
                        root = temp;
                }
                else
                {
                    Node temp = minValueNode(root.Right);
                    root.Book = temp.Book;
                    root.Right = Delete(root.Right, SortFunc(temp.Book));
                }
            }

            if (root == null) return root;

            // Update height of the current node
            root.Height = 1 + Math.Max(Height(root.Left), Height(root.Right));

            // Get the BF
            int balance = GetBalance(root);

            // Rebalance if not balanced
            if (balance > 1)
            {
                if (GetBalance(root.Left) < 0)
                    root.Left = LeftRotate(root.Left);
                return RightRotate(root);
            }

            if (balance < -1)
            {
                if (GetBalance(root.Right) > 0)
                    root.Right = RightRotate(root.Right);
                return LeftRotate(root);
            }

            return root;
        }


        Node minValueNode(Node node)
        {
            Node current = node;

            while (current.Left != null)
                current = current.Left;

            return current;
        }
       
    }

}
