﻿using FluxorLearning.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FluxorLearning.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class FeedbackController : ControllerBase
{
    [HttpPost]
    public void Post(UserFeedbackModel model)
    {
        var email = model.EmailAddress;
        var rating = model.Rating;
        var comment = model.Comment;

        Console.WriteLine($"Received rating {rating} from {email} with comment '{comment}'");
    }
}
