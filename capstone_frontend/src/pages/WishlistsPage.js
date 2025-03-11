import React, { useState, useEffect } from "react";
import axios from "axios";
import { Link } from "react-router-dom";

const WishlistsPage = () => {
  const [wishlists, setWishlists] = useState([]);

  useEffect(() => {
    const fetchWishlist = async () => {
      try {
        const token = localStorage.getItem("token");
        const response = await axios.get("http://localhost:8000/getwishlist", {
          headers: { Authorization: `Bearer ${token}` },
        });
        setWishlists(response.data);
      } catch (error) {
        console.error("Error fetching wishlist:", error);
      }
    };

    fetchWishlist();
  }, []);

  const removeFromWishlists = async (eventId) => {
    try {
      const token = localStorage.getItem("token");
      console.log("Sending delete wishlist request for eventId:", eventId);
      await axios.delete(`http://localhost:8000/deletewishlist/${eventId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      setWishlists(wishlists.filter((event) => event.eventId !== eventId));
    } catch (error) {
      console.error("Error removing from wishlist:", error);
    }
  };

  return (
    <div className="container">
      <h2>My Wishlist Events</h2>
      {wishlists.length === 0 ? (
        <p>No wishlist events yet.</p>
      ) : (
        <div className="event-list">
          {wishlists.map((event) => (
            <div key={event.eventId} className="event-card">
              <h3>{event.eventName}</h3>
              <p>{event.eventDate}</p>
              <a href={event.eventUrl} target="_blank" rel="noopener noreferrer">
              View Details
            </a>
              <br />
              <button onClick={() => removeFromWishlists(event.eventId)}>Remove</button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default WishlistsPage;