using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace lab2
{
	public class book 
	{
		public string year;
		public string author;
		public string name;
		public string genre;
		public string annotation;
		public book(string name,string author,int year,string genre,string annotation="")
		{
			this.name = name;
			this.author = author;
			this.year = year.ToString();
			this.genre = genre.ToString();
			this.annotation = annotation;
		}
	}
	public class HomeLib
	{
		private Dictionary <string,Dictionary <string,List<book>>> selector = new Dictionary <string,Dictionary <string,List<book>>>();
		private Dictionary <string,List<book>> find_by_name = new Dictionary <string,List<book>>();
		private Dictionary <string,List<book>> find_by_author = new Dictionary <string,List<book>>();
		private Dictionary <string,List<book>> find_by_year = new Dictionary <string,List<book>>();
		private Dictionary <string,List<book>> find_by_genre = new Dictionary <string,List<book>>();
		private Dictionary <string,book> find_by_id = new Dictionary <string,book>();
		private int max_id;
		public HomeLib()
		{
			this.max_id = 1;
			this.selector.Add ("name",this.find_by_name);
			this.selector.Add ("author",this.find_by_author);
			this.selector.Add ("year",this.find_by_year);
			this.selector.Add ("genre",this.find_by_genre);
		}
		public void add(book book)
		{
			if (!(this.find_by_name.ContainsKey (book.name))) {
				this.find_by_name.Add (book.name, new List<book> ());
			}
			if (!(this.find_by_author.ContainsKey (book.author))) {
				this.find_by_author.Add (book.author, new List<book> ());
			} 
			if (!(this.find_by_year.ContainsKey (book.year))) {
				this.find_by_year.Add (book.year, new List<book> ());
			} 
			if (!(this.find_by_genre.ContainsKey (book.genre))) {
				this.find_by_genre.Add (book.genre, new List<book> ());
			} 
			this.find_by_name [book.name].Add (book);
			this.find_by_author [book.author].Add (book);
			this.find_by_year [book.year].Add (book);
			this.find_by_genre [book.genre].Add (book);
			this.find_by_id.Add (this.max_id++.ToString (), book);
		}
		public List<book> select(string order)
		{
			Dictionary <string,string> final = new Dictionary <string,string>();
			var orders = new List<book>();
			List<Regex> patterns = new List<Regex> ();
			patterns.Add(new Regex(@"(author)\s+(\w+\s+\w+)"));
			patterns.Add(new Regex(@"(year)\s+(\d+)"));
			patterns.Add(new Regex(@"(genre)\s+(\S+\s*\S*)"));
			patterns.Add(new Regex(@"(name)\s+(.*)"));
			var years = new Regex (@"((years)\s+(\d+)-(\d+))");
			foreach (var regex in patterns) {
				if (regex.IsMatch (order)) {
					var key = regex.Match (order).Groups [1].Value;
					var val = regex.Match (order).Groups [2].Value;
					order = order.Replace (key, "");
					final.Add (key, val);
				}
			}
			foreach (var ord in final.Keys) {
				if (this.selector.ContainsKey (ord) && this.selector [ord].ContainsKey (final [ord])) {
					orders.AddRange (this.selector [ord] [final [ord]]);
					orders = orders.Intersect (this.selector [ord] [final [ord]]).ToList ();
				}
			}
			return orders;
		}
		private void premove(List<book> to_remove)
		{
			this.pshow_short(to_remove);
			Console.WriteLine ("are you sure u want to remove these books from your lib\n[y]es ,[n]o");
			if (Console.ReadKey ().ToString() == "y") 
			{
				foreach (var book in to_remove)
				{
					this.find_by_author [book.author].Remove (book);
					this.find_by_genre [book.genre].Remove (book);
					this.find_by_name [book.name].Remove (book);
					this.find_by_year [book.year].Remove (book);
				}
			}
		}
		public void remove(string order)
		{
			this.premove (this.select (order));
		}
		public void show_all()
		{
			List<List<book>> all = this.find_by_name.Values.ToList();
			foreach(var list in all)
			{
				this.pshow_short (list);
			}
		}
		public void show_short(string order)
		{
			this.pshow_short (this.select(order));
		}
		private void pshow_short(List<book> order_list)
		{
			string border = "#"+new string('-',Console.WindowWidth-2)+"#";
			if (order_list.Count > 0) 
			{
				Console.WriteLine (border);
				foreach (book book in order_list) 
				{
					Console.WriteLine (String.Format ("|name : {0}{1}\n|author : {2}{3}\n|year : {4}{5}\n|genre : {6}{7}",
						book.name, new String (' ', Console.WindowWidth - 9 - book.name.Length) + '|',
						book.author, new String (' ', Console.WindowWidth - 11 - book.author.Length) + '|',
						book.year, new String (' ', Console.WindowWidth - 9 - book.year.Length) + '|',
						book.genre, new String (' ', Console.WindowWidth - 10 - book.genre.Length) + '|'));
					Console.WriteLine (border);
				}
			}
			if (order_list.Count == 0) 
			{
				Console.WriteLine ("no books found by your order");
			}
		}
		public void greet()
		{
			Console.WriteLine ("Very wellcom to your marvelous library\nhow can i serve you?\n" +
				"available commands:\nremove - remove books bo specific order" +
				"\nsearch - show book's info by specific order\nshow_all - show alls book's info\nadd - add new book\n" +
				"help - show lit of commands\n");
		}
		public void perform()
		{
			string command = Console.ReadLine ();
			switch (command) 
			{
			case "remove":
				Console.WriteLine ("enter your order\n");
				this.remove (Console.ReadLine ());
				break;
			case "search":
				Console.WriteLine ("enter your order\n");
				this.show_short (Console.ReadLine ());
				break;
			case "show_all":
				this.show_all ();
				break;
			case "add":
				string name, author, genre;
				int year;
				Console.Write ("\nname: ");name = Console.ReadLine ();
				Console.Write ("\nauthor: ");author = Console.ReadLine ();
				Console.Write ("\nhenre: ");genre = Console.ReadLine ();
				Console.Write ("\nyear: ");Int32.TryParse(Console.ReadLine (),out year);
				this.add (new book(name,author,year,genre));
				break;
			case "help":
				this.greet ();
				break;
			default:
				break;
			}
		}
	}
	class MainClass
	{
		public static void Main (string[] args)
		{
			var lib = new HomeLib ();
			lib.add(new book("Animal Farm","George Orwell",1945,"satire"));
			lib.add(new book("1984","George Orwell",1948,"sci-fi"));
			lib.add(new book("Brave New World","Aldous Huxley",1931,"sci-fi"));
			lib.add(new book("The Call of Cthulhu","Howard Lovecraft",1928,"horror fiction"));
			lib.add(new book("The Shadow Over Innsmouth","Howard Lovecraft",1936,"horror fiction"));
			lib.add(new book("Dagon","Howard Lovecraft",1919,"horror fiction"));
			lib.greet ();
			while (true) 
			{
				lib.perform ();
			}
		}
	}
}