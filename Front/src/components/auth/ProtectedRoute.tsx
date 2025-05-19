import React, { useEffect } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import authService from '@/services/authService';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const location = useLocation();
  const isAuthenticated = authService.isAuthenticated();

  useEffect(() => {
    // Verificar se o token está expirado
    if (!isAuthenticated) {
      authService.logout();
    }
  }, [isAuthenticated]);

  if (!isAuthenticated) {
    // Redirecionar para a página de login, mantendo a URL original como state
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
