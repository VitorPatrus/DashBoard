﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI.Sistemas.Domain
{
    public class Gifs
    {
        public List<string> ListaGifs90 { get; set; }
        public List<string> ListaGifs50 { get; set; }
        public List<string> ListaGifs25 { get; set; }

        public Gifs(List<string> listaGifs90, List<string> listaGifs50, List<string> listaGifs25)
        {
            ListaGifs90 = new List<string>
        {
            "https://media4.giphy.com/media/v1.Y2lkPTc5MGI3NjExeHR2dzdndzZvOXZwbGl0bzAxeTQ4bWtjczE3Z3EyemtoMnJ3bTdhMCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/130do0lQXXcsla/giphy.gif",
            "https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExOTU5emNoeXZxOXRmMDRybjR1NjZ0MzdpNGZkaW1lbHJrYXBmMWRvYSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/xT5LMHxhOfscxPfIfm/giphy.gif",
            "https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExMWM1NWlld2hnZDA2ZThuMG9rdWJrNTFpajV6amxrbnNwNXQ0MzN3MCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/kEKcOWl8RMLde/giphy.gif",
            "https://media1.giphy.com/media/v1.Y2lkPTc5MGI3NjExb3N4ODI5NnR4MXV0bjFvcmNpdnp2bXFpZ3FwYTk4b2RkdmUyMHB6ayZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/aQwvKKi4Lv3t63nZl9/giphy.gif"
        };

            ListaGifs50 = new List<string>
        {
            "https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExdDlpZWQxc3liNTExOTl4NjJiMWM2dWZjZGdqODNldHg4aTVhejUzYiZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/eNTxLwTGW7E64/giphy.gif",
            "https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExbjBkbGk5NjNza216OW93YWR6Z29neGZjeHMxYzdpYjdrenpwd29nNSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/H5C8CevNMbpBqNqFjl/giphy.gif",
            "https://media4.giphy.com/media/v1.Y2lkPTc5MGI3NjExdGFrZXdtbHMzZzRoYmYxcDZlaHFjcjI3ajhhdDRidnNmMWdwZDRvMCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/4pMX5rJ4PYAEM/giphy.gif"
        };

            ListaGifs25 = new List<string>
        {
            "https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExdGltMnVqYXp6MzlpanFnem80Z2k3dTJ4ZDRzNXRzZjg0cGN3ZWlnNyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/MF0QiCa9JPEI6HiaCK/giphy.gif",
            "https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExd2tyZ2FnZTdraWRnY3B3YXcxandyeHkzcTVqa2JnNDNpYWM4eHp5ayZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/yr7n0u3qzO9nG/giphy.gif",
            "https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExZWk4MzVtOWYxY2l2NzB2aGVtZXlkMnJzN3QwZHB5dXkxbXl5MmJkMiZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/UKF08uKqWch0Y/giphy.gif"
        };
        }
        public string ListasGifs(List<string> gifs)
        {
            var random = new Random();
            int randomIndex = random.Next(0, gifs.Count);
            return gifs[randomIndex];
        }
    }
}
