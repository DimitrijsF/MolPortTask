using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Windows.Forms;

namespace MolPortTask
{
    public partial class Worker
    {
        protected MySqlConnection conn = new MySqlConnection("SERVER=localhost; Port=3306; DATABASE=molport; UID=root; PASSWORD=root;");
        public DataTable DoSearch(string text)
        {
            DataTable result = SelectTableSql(@"books.Name, books.Publisher, books.book_year as 'Book year', 
(select GROUP_CONCAT(authors.name) FROM authors join books_authors on authors.id = books_authors.author_id and books_authors.book_id = books.id where books_authors.book_id) as 'Authors'",
                "`books`", "",  
                "books.Name like '%" + text.ToLower() + "%' or books.publisher like '%" + text.ToLower() + "%' or books.book_year like '%" + text.ToLower() + "%' or Authors like '%" + text.ToLower() + "%'");
            return result;
        }

        public DataTable GetAuthors()
        {
            return SelectTableSql("`name`, `id`", "`authors`", "", "");
        }

        public long AddAuthor(string name)
        {
            return InsertCmd("`authors`", "`name`", "'" + name + "'");
        }

        public long AddBook(string name, string publisher, int year, int[] authors)
        {
            long id = InsertCmd("`books`", "`name`, `publisher`, `book_year`", "'" + name + "', '" + publisher + "', " + year);
            foreach(int aid in authors)
                InsertCmd("`books_authors`", "`book_id`, `author_id`", id.ToString() + ", " + aid.ToString());
            return id;
        }

        long InsertCmd(string table, string fields, string values)
        {
            MySqlCommand cmd = new MySqlCommand("INSERT INTO " + table + "(" + fields + ") values (" + values + ")", conn);
            while (conn.State == ConnectionState.Open)
                Application.DoEvents();
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            return cmd.LastInsertedId;
        }

        DataTable SelectTableSql(string query, string table, string statement, string having)
        {
            DataTable result = new DataTable();
            string request = "SELECT " + query + " FROM " + table;
            if (statement != "")
                request += " where " + statement;
            if (having != "")
                request += " HAVING " + having;
            MySqlCommand cmd = new MySqlCommand(request, conn);
            while (conn.State == ConnectionState.Open)
                Application.DoEvents();
            MySqlDataAdapter add = new MySqlDataAdapter(cmd);
            add.Fill(result);
            return result;
        }

        public DataTable GetAuthorSource ()
        {
            DataTable source = SelectTableSql("`name`, `id`", "`authors`", "", "");
            return source;
        }
    }
}
