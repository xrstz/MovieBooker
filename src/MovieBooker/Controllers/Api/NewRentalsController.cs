﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vidly.Dtos;
using Vidly.Models;

namespace Vidly.Controllers.Api
{
    public class NewRentalsController : ApiController
    {
        private ApplicationDbContext _context;
        public NewRentalsController()
        {
            this._context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }


        [HttpPost]
        public IHttpActionResult CreateNewRentals(NewRentalDto newRentals)
        {
            if (newRentals.MovieIds.Count == 0)
            {
                return BadRequest("No Movies Ids have been given.");
            }


            var customer = _context.Customers.SingleOrDefault(
                c => c.Id == newRentals.CustomerId
                );

            if (customer ==null)
            {
                return BadRequest("Customer is not valid.");
            }


            var movies = _context.Movies.Where(
                m => newRentals.MovieIds.Contains(m.Id)
                ).ToList();

            if (movies.Count != newRentals.MovieIds.Count)
            {
                return BadRequest("One or more MovieIds are invalid.");
            }

            foreach (var movie in movies)
            {
                if (movie.NumberAvailable == 0)
                {
                    return BadRequest("Movie is not available");
                }
                movie.NumberAvailable--;

                var rental = new Rental
                {
                    Customer = customer,
                    Movie = movie,
                    DateRented = DateTime.Now
                };


                _context.Rentals.Add(rental);

            }
            _context.SaveChanges();
            return Ok();

        }
    }
} 
