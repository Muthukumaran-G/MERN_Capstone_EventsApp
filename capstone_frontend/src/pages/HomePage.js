import React, { useState, useEffect } from "react";
import axios from "axios";

const HomePage = () => {
  const [events, setEvents] = useState([]);
  const [wishlist, setWishlist] = useState([]);
  const userEmail = localStorage.getItem("userEmail");
  const token = localStorage.getItem("token");

  useEffect(() => {
    const fetchEvents = async () => {
      try {
        const response = await axios.get("http://localhost:8000/events", {
          headers: { Authorization: `Bearer ${token}` },
        });
        setEvents(response.data);
      } catch (error) {
        console.error("Error fetching events:", error);
      }
    };

    const fetchWishlist = async () => {
      try {
        const response = await axios.get("http://localhost:8000/getwishlist", {
          headers: { Authorization: `Bearer ${token}` },
        });
        setWishlist(response.data);
      } catch (error) {
        console.error("Error fetching wishlist:", error);
      }
    };

    fetchEvents();
    fetchWishlist();
  }, []);

  const handleWishlistToggle = async (event) => {
    const isInWishlist = wishlist.some((item) => item.eventId === event.id);

    if (isInWishlist) {
      try {
        await axios.delete(`http://localhost:8000/deletewishlist/${event.id}`, {
          headers: { Authorization: `Bearer ${token}` },
        });

        setWishlist(wishlist.filter((item) => item.eventId !== event.id));
      } catch (error) {
        console.error("Error deleting from wishlist:", error);
      }
    } else {
      const wishlistItem = {
        userEmail,
        eventId: event.id,
        eventName: event.name,
        eventDate: event.date,
        eventUrl: event.url,
      };

      try {
        await axios.post("http://localhost:8000/addtowishlist", wishlistItem, {
          headers: { Authorization: `Bearer ${token}` },
        });

        setWishlist([...wishlist, wishlistItem]);
      } catch (error) {
        console.error("Error adding to wishlist:", error);
      }
    }
  };

  return (
    <div className="container">
      <h2>Upcoming Events</h2>
      <div className="event-list">
        {events.map((event) => (
          <div key={event.id} className="event-card">
            <img src={event.imageUrl} alt={event.name} />
            <h3>{event.name}</h3>
            <p>{event.date} - {event.time}</p>
            <a href={event.url} target="_blank" rel="noopener noreferrer">
              View Details
            </a>
            <br />
            <button onClick={() => handleWishlistToggle(event)} 
            style={{
              backgroundColor: wishlist.some((item) => item.eventId === event.id)
              ? "red": "blue",color: "white",}}>
                {wishlist.some((item) => item.eventId === event.id)
                ? "Delete from Wishlist": "Add to Wishlist"}
            </button>
          </div>
        ))}
      </div>
    </div>
  );
};

export default HomePage;
