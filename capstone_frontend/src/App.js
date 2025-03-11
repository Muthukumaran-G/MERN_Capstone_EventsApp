import React from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import HomePage from "./pages/HomePage";
import WishlistsPage from "./pages/WishlistsPage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import Header from "./components/Header"
import { LoadingProvider } from "./context/LoadingContext";
import useAxiosInterceptor from "./services/axiosConfig";
import LoadingOverlay from "./components/LoadingOverlay";

import "./App.css";

const PrivateRoute = ({ children }) => {
  const token = localStorage.getItem("token");
  return token ? children : <Navigate to="/login" />;
};

const App = () => {
  return (
    <LoadingProvider>
      <AxiosInterceptorWrapper />
      <Router>
      <LoadingOverlay />
       <Header />
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/" element={<PrivateRoute><HomePage /></PrivateRoute>} />
          <Route path="/wishlists" element={<PrivateRoute><WishlistsPage /></PrivateRoute>} />
        </Routes>
      </Router>
    </LoadingProvider>
  );
};
const AxiosInterceptorWrapper = () => {
  useAxiosInterceptor();
  return null;
};
export default App;