using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;
        public PhotoRepository(DataContext context)
        {
            _context = context;

        }

        // get single photo by id
        public async Task<Photo> PhotoByIdGetter(int photoId)
        {
            return await _context.Photos.IgnoreQueryFilters().SingleOrDefaultAsync(s=>s.Id == photoId);
        }

        // remove photo from DB
        public void PhotoRemove(Photo photo)
        {
            _context.Photos.Remove(photo);

        }

        // Get ALL unapproved photos
        public async Task<IEnumerable<ApprovePhotoDto>> PhotosUnapprovedDtoGetter()
        {
            // Create linq query against photos Dbset
            return await _context.Photos
            .IgnoreQueryFilters() // ignore global query filter -> to access unapproved photos
            .Where(x=>x.IsPhotoApproved == false) // only get unapproved photos 
            .Select(s=> new ApprovePhotoDto // select only the properties we want to return (which is shaped by ApprovePhotoDto)
            {
                // we create NEW Dto and return a list of these Dtos
                Id = s.Id, // return photo ID,not userID!
                photoUrl = s.Url,
                Username = s.AppUser.UserName,
                IsPhotoApproved = s.IsPhotoApproved
            })
            .ToListAsync(); // execute query against DB and return list of ApprovePhotoDto
        }
    }
}