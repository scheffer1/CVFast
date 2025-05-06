
import React from "react";
import { Link } from "react-router-dom";
import RegisterForm from "@/components/auth/RegisterForm";
import Navbar from "@/components/shared/Navbar";

const Register = () => {
  return (
    <div className="min-h-screen flex flex-col">
      <Navbar />
      <div className="flex-1 flex items-center justify-center py-12">
        <div className="w-full max-w-md px-4">
          <RegisterForm />
          <p className="text-center mt-6 text-gray-600">
            Already have an account?{" "}
            <Link to="/login" className="text-primary hover:underline">
              Login
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Register;
