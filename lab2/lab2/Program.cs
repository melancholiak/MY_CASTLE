﻿using System;
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
		public string defenition;
		public book(string name,string author,int year,string genre,string defenition="")
		{
			this.name = name;
			this.author = author;
			this.year = year.ToString();
			this.genre = genre.ToString();
			this.defenition = defenition;
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
			var years = new Regex (@"(years)\s+((\d+)-(\d+))");
			patterns.Add (years);
			if(years.IsMatch (order))
			{
				int begin=0, end=-1;
				Int32.TryParse (years.Match (order).Groups [3].Value,out begin);
				Int32.TryParse (years.Match (order).Groups [4].Value,out end);
				order.Replace (years.Match (order).Groups [1].Value,"");
				for (int i = begin; i < end; i++) 
				{
					orders.AddRange (this.select(order+"year "+i.ToString()));
				}
			}
			foreach (var regex in patterns) 
			{
				if(regex.IsMatch (order)) 
				{
					var key = regex.Match (order).Groups [1].Value;
					var val = regex.Match (order).Groups [2].Value;
					order = order.Replace (key, "");
					final.Add (key, val);
				}
			}
			foreach (var ord in final.Keys) 
			{
				if (this.selector.ContainsKey (ord)&&this.selector [ord].ContainsKey (final [ord]))
				{
					orders.AddRange (this.selector [ord] [final [ord]]);
					orders = orders.Intersect (this.selector [ord] [final [ord]]).ToList ();
				}
			}
			return orders;
		}
		public void show_short(string order)
		{
			int num = 1;
			string border = "#"+new string('-',Console.WindowWidth-2)+"#";
			var order_list = this.select (order);
			if (order_list.Count > 0) 
			{
				Console.WriteLine (border);
				foreach (book book in order_list) 
				{
					Console.WriteLine (String.Format("|{0:^}{1}|",num.ToString(),new string(' ',Console.WindowWidth-2-num++.ToString().Length)));
					Console.WriteLine (String.Format ("|name : {0}{1}\n|author : {2}{3}\n|year : {4}{5}\n|genre : {6}{7}",
						book.name, new String (' ', Console.WindowWidth - 9 - book.name.Length) + '|',
						book.author, new String (' ', Console.WindowWidth - 11 - book.author.Length) + '|',
						book.year, new String (' ', Console.WindowWidth - 9 - book.year.Length) + '|',
						book.genre, new String (' ', Console.WindowWidth - 10 - book.genre.Length) + '|'));
					Console.WriteLine (border);
				}
			}
		}
	}
	class MainClass
	{
		public static void Main (string[] args)
		{
			var lib = new HomeLib ();
			lib.add(new book("Animal Farm","George Orwell",1945,"satire",
				"Animal Farm is an allegorical and dystopian novella by George Orwell, first published in England on 17 August 1945. " +
				"According to Orwell, the book reflects events leading up to the Russian Revolution of 1917 and then on into the Stalinist" +
				" era of the Soviet Union.[1] Orwell, a democratic socialist,[2] was a critic of Joseph Stalin and hostile to Moscow-directed" +
				" Stalinism, an attitude that was critically shaped by his experiences during the Spanish Civil War.[3] The Soviet Union, he " +
				"believed, had become a brutal dictatorship, built upon a cult of personality and enforced by a reign of terror. In a letter " +
				"to Yvonne Davet, Orwell described Animal Farm as a satirical tale against Stalin (\"un conte satirique contre Staline\"),[4] " +
				"and in his essay \"Why I Write\" (1946), wrote that Animal Farm was the first book in which he tried, with full consciousness of " +
				"what he was doing, \"to fuse political purpose and artistic purpose into one whole\"."));
			lib.add(new book("1984","George Orwell",1948,"sci-fi"));
			lib.add(new book("Brave New World","Aldous Huxley",1931,"sci-fi"));
			lib.add(new book("The Call of Cthulhu","Howard Lovecraft",1928,"horror fiction"));
			lib.add(new book("The Shadow Over Innsmouth","Howard Lovecraft",1936,"horror fiction"));
			lib.add(new book("Dagon","Howard Lovecraft",1919,"horror fiction"));
			while (true) 
			{
				var order = Console.ReadLine ();
				lib.show_short (order);

			}
		}
	}
}