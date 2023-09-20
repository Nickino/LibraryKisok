// See https://aka.ms/new-console-template for more information
using System;
using LibraryKiosk;
using System.Globalization;
using System.IO;
using CsvHelper;

class Program
{
    static void Main()
    {
        AVLTree avlTree = new AVLTree();

        using (var reader = new StreamReader("books.csv"))
        using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false }))
        {
            var records = csv.GetRecords<Book>();

            foreach (var book in records)
            {
                avlTree.InsertBook(book);
            }
        }

        while (true)
        {
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1. Insert a new book(Check-in)");
            Console.WriteLine("2. Delete a book by title(Check-out)");
            Console.WriteLine("3. Sort the books by Title");
            Console.WriteLine("4. Sort the books by Author");
            Console.WriteLine("5. Sort the books by Publisher");
            Console.WriteLine("6. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    InsertBookToTree(avlTree);
                    break;
                case "2":
                    DeleteBookFromTree(avlTree);
                    break;
                case "3":
                    BuildTreeSortedByKey(avlTree, book => book.Title);
                    Console.WriteLine("\nBooks sorted by Title: \n");
                    avlTree.InOrderTraversal(avlTree.root);
                    break;
                case "4":
                    //if author is null, sort by title after the ones that have author
                    BuildTreeSortedByKey(avlTree, book => book.Author, book => book.Title);
                    Console.WriteLine("\nBooks sorted by Author: \n");
                    avlTree.InOrderTraversal(avlTree.root);
                    break;
                case "5":
                    //same as sorting by author
                    BuildTreeSortedByKey(avlTree, book => book.Publisher, book => book.Title);
                    Console.WriteLine("\nBooks sorted by Publisher: \n");
                    avlTree.InOrderTraversal(avlTree.root);
                    break;
                case "6":
                    Console.WriteLine("Exit");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please select again.");
                    break;
            }
        }
    }

    private static void BuildTreeSortedByKey(AVLTree currentTree, Func<Book, string> primarySortingKey, Func<Book, string> secondarySortingKey = null)
    {
        currentTree.ChangeSortingKey(primarySortingKey, secondarySortingKey);
    }



    private static void InsertBookToTree(AVLTree avlTree)
    {
        Console.WriteLine("Enter book title:");
        string title = Console.ReadLine();

        Console.WriteLine("Enter book author:");
        string author = Console.ReadLine();

        //page number should be an interger
        Console.WriteLine("Enter number of pages:");
        if (!int.TryParse(Console.ReadLine(), out int pages))
        {
            Console.WriteLine("Invalid number of pages. Insertion cancelled.");
            return;
        }

        Console.WriteLine("Enter book publisher:");
        string publisher = Console.ReadLine();

        Book newBook = new Book(title, author, pages, publisher);
        avlTree.AllBooks.Add(newBook);
        avlTree.InsertBook(newBook);
        Console.WriteLine($"Book \"{title}\" inserted successfully!");
    }

    private static void DeleteBookFromTree(AVLTree avlTree)
    {
        Console.WriteLine("Enter the title of the book to delete:");
        string title = Console.ReadLine();

        if (avlTree.Search(avlTree.root, title) != null)
        {
            avlTree.DeleteBookByTitle(title);
            Console.WriteLine($"Book \"{title}\" deleted");
        }
        else
        {
            Console.WriteLine($"Book \"{title}\" not found.");
        }
    }

    

}