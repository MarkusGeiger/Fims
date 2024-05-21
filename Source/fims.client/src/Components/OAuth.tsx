// // BFF/ClientApp/src/context/AuthContext.js

// import React, { useState, useEffect, useContext } from "react";
// import { Link, Navigate } from "react-router-dom";

// export interface OAuthContextType {
//   isAuthenticated: boolean | undefined;
//   user: any;
//   isLoading: boolean;
//   login: any;
//   logout: any;
// }

// export const AuthContext = React.createContext<OAuthContextType>(null!);
// export const useAuth = () => useContext(AuthContext);
// export function AuthProvider(props: { children: React.ReactNode }) {
//     const [isAuthenticated, setIsAuthenticated] = useState<boolean>();
//     const [user, setUser] = useState();
//     const [isLoading, setIsLoading] = useState(false);

//     const getUser = async () => {
//         const response = await fetch('/api/auth/getUser');
//         const json = await response.json();

//         setIsAuthenticated(json.isAuthenticated);
//         setIsLoading(false);
//         if (json.isAuthenticated) setUser(json.claims);
//     }

//     useEffect(() => {
//         getUser();
//     }, []);

//     const login = () => {
//         window.location.href = '/api/auth/login';
//     }

//     const logout = () => {
//         window.location.href = '/api/auth/logout';
//     }

//     return (
//         <AuthContext.Provider
//             value={{
//                 isAuthenticated,
//                 user,
//                 isLoading,
//                 login,
//                 logout
//             }}
//         >
//             {props.children}
//         </AuthContext.Provider>
//     );
// };

// export function OAuth(){
//   return (
//   <AuthProvider>
//     <Navigate to="/login"/>
//     <Navigate to="/logout"/>
//     <OAuthContent/>
//   </AuthProvider>);
// }

// export function OAuthContent(){
//   const oauthContext  = useAuth();
//   return (
//   <>
//     <p>{oauthContext.isAuthenticated ? "Hello Auth0!" : "Access Denied!"}</p>
//     <p>{oauthContext.isLoading ? "Loading..." : "Loaded!"}</p>
//   </>);
// }