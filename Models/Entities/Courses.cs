﻿using System;
using System.Collections.Generic;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.Entities
{
    public partial class Course
    {
        public Course(string title, string author)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Manca il titolo");
            }
            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException("Manca l'autore");
            }

            Title = title;
            Author = author;
            Lessons = new HashSet<Lesson>();
            CurrentPrice = new Money(Currency.EUR, 0);
            FullPrice = new Money(Currency.EUR, 0);
            ImagePath = "/Courses/default.png";
        }

        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Author { get; private set; }
        public string Email { get; private set; }
        public double Rating { get; private set; }
        public Money FullPrice { get; private set; }
        public Money CurrentPrice { get; private set; }

        public virtual ICollection<Lesson> Lessons { get; private set; }


        public void ChangeTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
            {
                throw new ArgumentException("Manca il titolo");
            }
            Title = newTitle;
        }

        public void ChangePrices(Money newFullPrice, Money newDiscountPrice)
        {
            if (newFullPrice == null || newDiscountPrice == null)
            {
                throw new ArgumentException("I valori non possono essere nulli");
            }
            if (newFullPrice.Currency != newDiscountPrice.Currency)
            {
                throw new ArgumentException("Le valute non corrispondono");
            }
            if (newFullPrice.Amount < newDiscountPrice.Amount )
            {
                throw new ArgumentException("Il prezzo corrente deve essere superiore a quello scontato");
            }
            FullPrice = newFullPrice;
            CurrentPrice = newDiscountPrice;
        }
    }
}
