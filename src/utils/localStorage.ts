
import { User, Resume } from "@/types";

// Users
export const saveUser = (user: User): void => {
  localStorage.setItem("current_user", JSON.stringify(user));
};

export const getUser = (): User | null => {
  const user = localStorage.getItem("current_user");
  return user ? JSON.parse(user) : null;
};

export const removeUser = (): void => {
  localStorage.removeItem("current_user");
};

// Users database (for demo purposes)
export const saveUserToDb = (user: User): void => {
  const users = getUsersFromDb();
  users.push(user);
  localStorage.setItem("users", JSON.stringify(users));
};

export const getUsersFromDb = (): User[] => {
  const users = localStorage.getItem("users");
  return users ? JSON.parse(users) : [];
};

export const findUserByEmail = (email: string): User | undefined => {
  const users = getUsersFromDb();
  return users.find((user) => user.email === email);
};

// Resumes
export const saveResume = (resume: Resume): void => {
  const resumes = getResumes();
  const existingIndex = resumes.findIndex(r => r.id === resume.id);
  
  if (existingIndex !== -1) {
    resumes[existingIndex] = resume;
  } else {
    resumes.push(resume);
  }
  
  localStorage.setItem("resumes", JSON.stringify(resumes));
};

export const getResumes = (): Resume[] => {
  const resumes = localStorage.getItem("resumes");
  return resumes ? JSON.parse(resumes) : [];
};

export const getResumesByUserId = (userId: string): Resume[] => {
  const resumes = getResumes();
  return resumes.filter(resume => resume.userId === userId);
};

export const getResumeById = (id: string): Resume | undefined => {
  const resumes = getResumes();
  return resumes.find(resume => resume.id === id);
};

export const getResumeByShareableLink = (link: string): Resume | undefined => {
  const resumes = getResumes();
  return resumes.find(resume => resume.shareableLink === link);
};

export const deleteResume = (id: string): void => {
  const resumes = getResumes();
  const updatedResumes = resumes.filter(resume => resume.id !== id);
  localStorage.setItem("resumes", JSON.stringify(updatedResumes));
};
