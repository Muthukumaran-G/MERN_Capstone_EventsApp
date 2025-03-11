import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import axios from "axios";
import MockAdapter from "axios-mock-adapter";
import LoginPage from "../src/pages/LoginPage";

const mockAxios = new MockAdapter(axios);

describe("LoginPage Component", () => {
  beforeEach(() => {
    mockAxios.reset();
  });

  test("renders login form", () => {
    render(
      <MemoryRouter>
        <LoginPage />
      </MemoryRouter>
    );

    expect(screen.getByPlaceholderText(/Email/i)).toBeInTheDocument();
    expect(screen.getByPlaceholderText(/Password/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Login/i })).toBeInTheDocument();
  });

  test("displays error message on failed login", async () => {
    mockAxios.onPost("http://localhost:8000/login").reply(401);

    render(
      <MemoryRouter>
        <LoginPage />
      </MemoryRouter>
    );

    fireEvent.change(screen.getByPlaceholderText(/Email/i), { target: { value: "asd@example.com" } });
    fireEvent.change(screen.getByPlaceholderText(/Password/i), { target: { value: "asd4636" } });
    fireEvent.click(screen.getByRole("button", { name: /Login/i }));

    expect(await screen.findByText(/Invalid credentials/i)).toBeInTheDocument();
  });

  test("successful login stores token and redirects", async () => {
    const mockToken = "mocked_jwt_token";
    mockAxios.onPost("http://localhost:8000/login").reply(200, mockToken);

    Storage.prototype.setItem = jest.fn();

    render(
      <MemoryRouter>
        <LoginPage />
      </MemoryRouter>
    );

    fireEvent.change(screen.getByPlaceholderText(/Email/i), { target: { value: "user@example.com" } });
    fireEvent.change(screen.getByPlaceholderText(/Password/i), { target: { value: "tdeegfdyfhg" } });
    fireEvent.click(screen.getByRole("button", { name: /Login/i }));

    await new Promise((resolve) => setTimeout(resolve, 100));
    expect(localStorage.setItem).toHaveBeenCalledWith("token", mockToken);
    expect(localStorage.setItem).toHaveBeenCalledWith("userEmail", "user@example.com");
  });
});
