using JamalKhanah.Core;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.ChatAndNotification;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;


namespace JamalKhanah.BusinessLayer.SignalR;

//  [Authorize]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
    private readonly ApplicationContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatHub(UserManager<ApplicationUser> userManager, ApplicationContext context)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async void SendChatMessage(string receiver, string message)
    {
        if (Context.User != null)
        {
            var senderId = Context.User.Claims.First(i => i.Type == "uid").Value;
            var sender = _userManager.FindByIdAsync(senderId).Result.UserName;
            var receiveUserId = _userManager.FindByNameAsync(receiver).Result.Id;

            if (senderId == null && receiveUserId == null)
                throw new MemberAccessException();

            MessageChat messageChat = new MessageChat
            {
                SendUserId = senderId,
                ReceiveUserId = receiveUserId,
                Message = message,
                MessageDate = DateTime.Now
            };
            _context.MessageChats.Add(messageChat);
            await _context.SaveChangesAsync();

            foreach (var connectionId in _context.UserConnections.Where(x => x.UserName == receiver).Select(x => x.Connection).ToList())
            {
                await Clients.Client(connectionId).SendAsync("ReceiveOneMessage", sender, message);
            }
        }
    }

    public override Task OnConnectedAsync()
    {
        if (Context.User != null)
        {
            var id = Context.User.Claims.First(i => i.Type == "uid").Value;
            UserConnection userConnection = new UserConnection
            {
                Connection = Context.ConnectionId,
                UserName = _userManager.FindByIdAsync(id).Result.UserName,
                ConnectionTime = DateTime.Now
            };
            _context.UserConnections.Add(userConnection);
        }

        _context.SaveChanges();

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception stopCalled)
    {
        if (Context.User != null)
        {
            var id = Context.User.Claims.First(i => i.Type == "uid").Value;
            string name = _userManager.FindByIdAsync(id).Result.UserName;

            foreach (var connection in _context.UserConnections.Where(x => x.UserName == name).ToList())
            {
                _context.UserConnections.Remove(connection);
                _context.SaveChanges();
            }
        }

        return base.OnDisconnectedAsync(stopCalled);
    }
}