using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Models
{
	[Table("Comments")]
	public class Comment
	{
		[Key]
		[Required]
		public int CommentID { get; set; }

		public bool Deleted { get; set; }

		public Route Route { get; set; }

		[ForeignKey("ID")]
		public int RouteID { get; set; }

		/// <summary>
		/// The ID of the comment that this comment is a reply to. Null if the comment is a root-level comment.
		/// </summary>
		public int OriginalPostID { get; set; }

		public string ApplicationUserRefId { get; set; }

		public ApplicationUser User { get; set; }

		public DateTime Date { get; set; }

		public DateTime DeletionDate { get; set; }

		public string Content { get; set; }
	}
}