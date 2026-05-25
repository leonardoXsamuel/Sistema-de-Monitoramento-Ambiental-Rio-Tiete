import http from './http';
import type { FileTransfer } from '../types/types';

export const uploadFiles = (files: File[], roomId: number): Promise<FileTransfer[]> => {
  const form = new FormData();
  files.forEach((f) => form.append('files', f));
  form.append('roomId', roomId.toString());
  return http
    .post<FileTransfer[]>('/api/files/upload', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    .then((r) => r.data);
};

export const getFilesByRoom = async (roomId: number): Promise<FileTransfer[]> => {
  try {
    const res = await http.get<FileTransfer[]>(`/api/files/room/${roomId}`);
    return res.data;
  } catch (err: any) {
    if (err.response?.status === 404) return [];
    throw err;
  }
};
