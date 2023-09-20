using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryKiosk
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Pages { get; set; }
        public string Publisher { get; set; }

        public Book(string title, string author, int pages, string publisher)
        {
            Title = title;
            Author = author;
            Pages = pages;
            Publisher = publisher;
        }

       
        public void Print()
        {
            Console.WriteLine($"Title: {Title}, Author: {Author}, Pages: {Pages}, Publisher: {Publisher}");
        }
    }
}
